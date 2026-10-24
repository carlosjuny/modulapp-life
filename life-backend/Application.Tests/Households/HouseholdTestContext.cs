using Application.Abstractions;
using Application.EventsLog;
using Application.Households;
using Domain.Entities;
using Domain.Enums;
using NSubstitute;

namespace Application.Tests.Households;

public class HouseholdTestContext
{
    public FakeHouseholdStore Store { get; } = new();

    public FakeClock Clock { get; } = new();

    public HouseholdService Service { get; }

    public HouseholdTestContext()
    {
        Service = new HouseholdService(Store, new EventLogService(Substitute.For<IEventLogStore>()), Clock);
    }

    public User AddUser(string name)
    {
        var user = new User { Name = name, Email = $"{name.ToLowerInvariant()}@example.com" };
        Store.Users.Add(user);
        return user;
    }

    public async Task<Guid> CreateHouseholdAsync(User admin)
    {
        var result = await Service.CreateAsync(admin.Id, "Casa");
        return result.Value!.Id;
    }

    public async Task JoinAsync(Guid householdId, User admin, User member)
    {
        var invitation = await Service.CreateInvitationAsync(admin.Id, householdId);
        await Service.AcceptInvitationAsync(member.Id, invitation.Value!.Code);
    }

    public Membership MembershipOf(User user, Guid householdId)
        => Store.Memberships.Single(m => m.UserId == user.Id && m.HouseholdId == householdId);
}

public class FakeClock : TimeProvider
{
    private DateTimeOffset _now = new(2026, 10, 24, 12, 0, 0, TimeSpan.Zero);

    public void Advance(TimeSpan time) => _now += time;

    public override DateTimeOffset GetUtcNow() => _now;
}

public class FakeHouseholdStore : IHouseholdStore
{
    public List<User> Users { get; } = [];

    public List<Household> Households { get; } = [];

    public List<Membership> Memberships { get; } = [];

    public List<Invitation> Invitations { get; } = [];

    public Task<User?> FindUserAsync(Guid userId, CancellationToken cancellationToken)
        => Task.FromResult(Users.FirstOrDefault(u => u.Id == userId));

    public Task<Membership?> FindMembershipAsync(Guid userId, Guid householdId, CancellationToken cancellationToken)
        => Task.FromResult(Memberships.FirstOrDefault(m => m.UserId == userId && m.HouseholdId == householdId));

    public Task<IReadOnlyList<Membership>> ListActiveMembershipsOfUserAsync(Guid userId, CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyList<Membership>>(
            Memberships.Where(m => m.UserId == userId && m.Status == MembershipStatus.Active).ToList());

    public Task<IReadOnlyList<Membership>> ListActiveMembersAsync(Guid householdId, CancellationToken cancellationToken)
    {
        var members = Memberships.Where(m => m.HouseholdId == householdId && m.Status == MembershipStatus.Active).ToList();
        foreach (var member in members)
        {
            member.User ??= Users.First(u => u.Id == member.UserId);
        }

        return Task.FromResult<IReadOnlyList<Membership>>(members);
    }

    public Task<int> CountActiveAdminsAsync(Guid householdId, CancellationToken cancellationToken)
        => Task.FromResult(Memberships.Count(m => m.HouseholdId == householdId
            && m.Status == MembershipStatus.Active
            && m.Role is MembershipRole.Admin or MembershipRole.Owner));

    public Task<Invitation?> FindInvitationAsync(string code, CancellationToken cancellationToken)
    {
        var invitation = Invitations.FirstOrDefault(i => i.Code == code);
        if (invitation is not null)
        {
            invitation.Household ??= Households.FirstOrDefault(h => h.Id == invitation.HouseholdId);
        }

        return Task.FromResult(invitation);
    }

    public void Add<TEntity>(TEntity entity) where TEntity : class
    {
        switch (entity)
        {
            case Household household:
                Households.Add(household);
                foreach (var membership in household.Memberships)
                {
                    membership.HouseholdId = household.Id;
                    membership.Household = household;
                    Memberships.Add(membership);
                }

                break;
            case Membership membership:
                membership.Household ??= Households.FirstOrDefault(h => h.Id == membership.HouseholdId);
                Memberships.Add(membership);
                break;
            case Invitation invitation:
                Invitations.Add(invitation);
                break;
        }
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
