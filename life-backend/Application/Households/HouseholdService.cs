using System.Security.Cryptography;
using Application.Abstractions;
using Application.EventsLog;
using Domain.Entities;
using Domain.Enums;

namespace Application.Households;

public class HouseholdService(IHouseholdStore store, EventLogService eventLog, TimeProvider clock)
{
    public const int MaxNameLength = 150;
    public static readonly TimeSpan InvitationLifetime = TimeSpan.FromDays(7);

    private const string InvalidInvitationMessage = "El código de invitación no es válido, expiró o ya fue usado.";
    private const string NotMemberMessage = "No pertenecés a este hogar.";
    private const string AdminOnlyMessage = "Solo el administrador del hogar puede realizar esta acción.";
    private const string UserNotFoundMessage = "Usuario no encontrado.";
    private const string TargetNotMemberMessage = "El miembro no pertenece al hogar.";

    public async Task<HouseholdResult<HouseholdSummary>> CreateAsync(Guid userId, string? name, CancellationToken cancellationToken = default)
    {
        var trimmed = (name ?? string.Empty).Trim();
        if (trimmed.Length is 0 or > MaxNameLength)
        {
            return HouseholdResult<HouseholdSummary>.Fail(HouseholdStatus.ValidationFailed, $"El nombre del hogar es obligatorio y no puede superar {MaxNameLength} caracteres.");
        }

        var user = await store.FindUserAsync(userId, cancellationToken);
        if (user is null)
        {
            return HouseholdResult<HouseholdSummary>.Fail(HouseholdStatus.NotFound, UserNotFoundMessage);
        }

        var household = new Household { Name = trimmed, CreatedByUserId = userId };
        household.Memberships.Add(new Membership { UserId = userId, Role = MembershipRole.Admin, Status = MembershipStatus.Active });
        store.Add(household);
        user.ActiveHouseholdId = household.Id;
        await store.SaveChangesAsync(cancellationToken);

        return HouseholdResult<HouseholdSummary>.Success(new HouseholdSummary(household.Id, household.Name, nameof(MembershipRole.Admin), true));
    }

    public async Task<HouseholdResult<IReadOnlyList<HouseholdSummary>>> ListMineAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await store.FindUserAsync(userId, cancellationToken);
        if (user is null)
        {
            return HouseholdResult<IReadOnlyList<HouseholdSummary>>.Fail(HouseholdStatus.NotFound, UserNotFoundMessage);
        }

        var memberships = await store.ListActiveMembershipsOfUserAsync(userId, cancellationToken);
        IReadOnlyList<HouseholdSummary> items = memberships
            .Select(m => new HouseholdSummary(m.HouseholdId, m.Household!.Name, m.Role.ToString(), m.HouseholdId == user.ActiveHouseholdId))
            .ToList();

        return HouseholdResult<IReadOnlyList<HouseholdSummary>>.Success(items);
    }

    public async Task<HouseholdResult<Unit>> SetActiveAsync(Guid userId, Guid householdId, CancellationToken cancellationToken = default)
    {
        if (await ActiveMembershipAsync(userId, householdId, cancellationToken) is null)
        {
            return HouseholdResult<Unit>.Fail(HouseholdStatus.Forbidden, NotMemberMessage);
        }

        var user = await store.FindUserAsync(userId, cancellationToken);
        if (user is null)
        {
            return HouseholdResult<Unit>.Fail(HouseholdStatus.NotFound, UserNotFoundMessage);
        }

        user.ActiveHouseholdId = householdId;
        await store.SaveChangesAsync(cancellationToken);

        return HouseholdResult<Unit>.Success(Unit.Value);
    }

    public async Task<HouseholdResult<InvitationSummary>> CreateInvitationAsync(Guid userId, Guid householdId, CancellationToken cancellationToken = default)
    {
        var membership = await ActiveMembershipAsync(userId, householdId, cancellationToken);
        if (membership is null)
        {
            return HouseholdResult<InvitationSummary>.Fail(HouseholdStatus.Forbidden, NotMemberMessage);
        }

        if (!IsAdmin(membership))
        {
            return HouseholdResult<InvitationSummary>.Fail(HouseholdStatus.Forbidden, AdminOnlyMessage);
        }

        var invitation = new Invitation
        {
            HouseholdId = householdId,
            CreatedByUserId = userId,
            Code = Convert.ToHexString(RandomNumberGenerator.GetBytes(16)).ToLowerInvariant(),
            ExpiresAt = clock.GetUtcNow().UtcDateTime.Add(InvitationLifetime)
        };
        store.Add(invitation);
        await store.SaveChangesAsync(cancellationToken);

        return HouseholdResult<InvitationSummary>.Success(new InvitationSummary(householdId, invitation.Code, invitation.ExpiresAt));
    }

    public async Task<HouseholdResult<HouseholdSummary>> AcceptInvitationAsync(Guid userId, string? code, CancellationToken cancellationToken = default)
    {
        var invitation = string.IsNullOrWhiteSpace(code)
            ? null
            : await store.FindInvitationAsync(code.Trim().ToLowerInvariant(), cancellationToken);

        var now = clock.GetUtcNow().UtcDateTime;
        if (invitation?.Household is null || !invitation.IsAvailable(now))
        {
            return HouseholdResult<HouseholdSummary>.Fail(HouseholdStatus.InvalidInvitation, InvalidInvitationMessage);
        }

        var user = await store.FindUserAsync(userId, cancellationToken);
        if (user is null)
        {
            return HouseholdResult<HouseholdSummary>.Fail(HouseholdStatus.NotFound, UserNotFoundMessage);
        }

        var membership = await store.FindMembershipAsync(userId, invitation.HouseholdId, cancellationToken);
        if (membership is { Status: MembershipStatus.Active })
        {
            return HouseholdResult<HouseholdSummary>.Fail(HouseholdStatus.Conflict, "Ya pertenecés a este hogar.");
        }

        if (membership is null)
        {
            membership = new Membership { UserId = userId, HouseholdId = invitation.HouseholdId };
            store.Add(membership);
        }

        membership.Role = MembershipRole.Member;
        membership.Status = MembershipStatus.Active;
        invitation.UsedAt = now;
        invitation.UsedByUserId = userId;
        user.ActiveHouseholdId = invitation.HouseholdId;
        await store.SaveChangesAsync(cancellationToken);

        return HouseholdResult<HouseholdSummary>.Success(
            new HouseholdSummary(invitation.HouseholdId, invitation.Household.Name, nameof(MembershipRole.Member), true));
    }

    public async Task<HouseholdResult<IReadOnlyList<MemberSummary>>> ListMembersAsync(Guid userId, Guid householdId, CancellationToken cancellationToken = default)
    {
        if (await ActiveMembershipAsync(userId, householdId, cancellationToken) is null)
        {
            return HouseholdResult<IReadOnlyList<MemberSummary>>.Fail(HouseholdStatus.Forbidden, NotMemberMessage);
        }

        var members = await store.ListActiveMembersAsync(householdId, cancellationToken);
        IReadOnlyList<MemberSummary> items = members
            .Select(m => new MemberSummary(m.UserId, m.User!.Name, m.User.Email, m.Role.ToString(), m.CreatedAt))
            .ToList();

        return HouseholdResult<IReadOnlyList<MemberSummary>>.Success(items);
    }

    public async Task<HouseholdResult<Unit>> RemoveMemberAsync(Guid userId, Guid householdId, Guid targetUserId, CancellationToken cancellationToken = default)
    {
        var requester = await ActiveMembershipAsync(userId, householdId, cancellationToken);
        if (requester is null)
        {
            return HouseholdResult<Unit>.Fail(HouseholdStatus.Forbidden, NotMemberMessage);
        }

        if (!IsAdmin(requester))
        {
            return HouseholdResult<Unit>.Fail(HouseholdStatus.Forbidden, AdminOnlyMessage);
        }

        if (targetUserId == userId)
        {
            return HouseholdResult<Unit>.Fail(HouseholdStatus.ValidationFailed, "Para salir del hogar usá la opción de salir.");
        }

        var target = await ActiveMembershipAsync(targetUserId, householdId, cancellationToken);
        if (target is null)
        {
            return HouseholdResult<Unit>.Fail(HouseholdStatus.NotFound, TargetNotMemberMessage);
        }

        if (IsAdmin(target))
        {
            return HouseholdResult<Unit>.Fail(HouseholdStatus.Conflict, "No se puede expulsar a un administrador.");
        }

        await DeactivateAsync(target, householdId, cancellationToken);
        await eventLog.RecordAsync(householdId, userId, EventLogType.MemberRemoved,
            new Dictionary<string, string?> { ["userId"] = targetUserId.ToString(), ["reason"] = "removed" }, cancellationToken);

        return HouseholdResult<Unit>.Success(Unit.Value);
    }

    public async Task<HouseholdResult<Unit>> LeaveAsync(Guid userId, Guid householdId, CancellationToken cancellationToken = default)
    {
        var membership = await ActiveMembershipAsync(userId, householdId, cancellationToken);
        if (membership is null)
        {
            return HouseholdResult<Unit>.Fail(HouseholdStatus.Forbidden, NotMemberMessage);
        }

        if (IsAdmin(membership) && await store.CountActiveAdminsAsync(householdId, cancellationToken) <= 1)
        {
            return HouseholdResult<Unit>.Fail(HouseholdStatus.Conflict, "El último administrador no puede salir del hogar sin transferir la administración.");
        }

        await DeactivateAsync(membership, householdId, cancellationToken);
        await eventLog.RecordAsync(householdId, userId, EventLogType.MemberRemoved,
            new Dictionary<string, string?> { ["userId"] = userId.ToString(), ["reason"] = "left" }, cancellationToken);

        return HouseholdResult<Unit>.Success(Unit.Value);
    }

    public async Task<HouseholdResult<Unit>> TransferAdminAsync(Guid userId, Guid householdId, Guid targetUserId, CancellationToken cancellationToken = default)
    {
        var requester = await ActiveMembershipAsync(userId, householdId, cancellationToken);
        if (requester is null)
        {
            return HouseholdResult<Unit>.Fail(HouseholdStatus.Forbidden, NotMemberMessage);
        }

        if (!IsAdmin(requester))
        {
            return HouseholdResult<Unit>.Fail(HouseholdStatus.Forbidden, AdminOnlyMessage);
        }

        if (targetUserId == userId)
        {
            return HouseholdResult<Unit>.Fail(HouseholdStatus.ValidationFailed, "Elegí a otro miembro para transferir la administración.");
        }

        var target = await ActiveMembershipAsync(targetUserId, householdId, cancellationToken);
        if (target is null)
        {
            return HouseholdResult<Unit>.Fail(HouseholdStatus.NotFound, TargetNotMemberMessage);
        }

        target.Role = MembershipRole.Admin;
        requester.Role = MembershipRole.Member;
        await store.SaveChangesAsync(cancellationToken);
        await eventLog.RecordAsync(householdId, userId, EventLogType.RoleChanged,
            new Dictionary<string, string?> { ["userId"] = targetUserId.ToString(), ["role"] = nameof(MembershipRole.Admin) }, cancellationToken);

        return HouseholdResult<Unit>.Success(Unit.Value);
    }

    private async Task<Membership?> ActiveMembershipAsync(Guid userId, Guid householdId, CancellationToken cancellationToken)
    {
        var membership = await store.FindMembershipAsync(userId, householdId, cancellationToken);
        return membership is { Status: MembershipStatus.Active } ? membership : null;
    }

    private static bool IsAdmin(Membership membership) => membership.Role is MembershipRole.Admin or MembershipRole.Owner;

    private async Task DeactivateAsync(Membership membership, Guid householdId, CancellationToken cancellationToken)
    {
        membership.Status = MembershipStatus.Removed;

        var user = await store.FindUserAsync(membership.UserId, cancellationToken);
        if (user?.ActiveHouseholdId == householdId)
        {
            user.ActiveHouseholdId = null;
        }

        await store.SaveChangesAsync(cancellationToken);
    }
}
