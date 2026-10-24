using Application.Abstractions;
using Domain.Entities;
using Domain.Enums;

namespace Application.Account;

public enum AccountStatus
{
    Success,
    NotFound
}

public class AccountService(IAccountStore store)
{
    public const string DeletedUserName = "Usuario eliminado";

    public static string AnonymizedEmail(Guid userId) => $"deleted-{userId:N}@deleted.invalid";

    public async Task<AccountStatus> DeleteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await store.FindUserAsync(userId, cancellationToken);
        if (user is null)
        {
            return AccountStatus.NotFound;
        }

        foreach (var membership in await store.ListActiveMembershipsOfUserAsync(userId, cancellationToken))
        {
            await LeaveHouseholdAsync(userId, membership, cancellationToken);
        }

        await store.RemoveSignInDataAsync(userId, cancellationToken);

        user.Email = AnonymizedEmail(userId);
        user.Name = DeletedUserName;
        user.AvatarUrl = null;
        user.ActiveHouseholdId = null;
        await store.SaveChangesAsync(cancellationToken);

        return AccountStatus.Success;
    }

    private async Task LeaveHouseholdAsync(Guid userId, Membership membership, CancellationToken cancellationToken)
    {
        var members = await store.ListActiveMembersAsync(membership.HouseholdId, cancellationToken);
        var others = members.Where(m => m.UserId != userId).ToList();

        if (others.Count == 0)
        {
            store.RemoveHousehold(membership.Household!);

            return;
        }

        var isOnlyAdmin = IsAdmin(membership) && !others.Any(IsAdmin);
        if (isOnlyAdmin)
        {
            others.OrderBy(m => m.CreatedAt).First().Role = MembershipRole.Admin;
        }

        membership.Status = MembershipStatus.Removed;
    }

    private static bool IsAdmin(Membership membership)
        => membership.Role is MembershipRole.Admin or MembershipRole.Owner;
}
