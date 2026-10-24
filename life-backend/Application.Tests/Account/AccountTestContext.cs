using Application.Abstractions;
using Application.Account;
using Domain.Entities;
using Domain.Enums;

namespace Application.Tests.Account;

public class AccountTestContext
{
    private DateTime _joinedAt = new(2026, 10, 24, 12, 0, 0, DateTimeKind.Utc);

    public FakeAccountStore Store { get; } = new();

    public AccountService Service { get; }

    public AccountTestContext()
    {
        Service = new AccountService(Store);
    }

    public User AddUser(string name)
    {
        var user = new User { Name = name, Email = $"{name.ToLowerInvariant()}@example.com", AvatarUrl = "https://example.com/a.png" };
        Store.Users.Add(user);
        Store.Credentials.Add(new UserCredential { UserId = user.Id, PasswordHash = "hash" });
        Store.ExternalLogins.Add(new ExternalLogin { UserId = user.Id, Provider = "Google", Subject = $"sub-{name}" });
        Store.RefreshTokens.Add(new RefreshToken { UserId = user.Id, TokenHash = $"hash-{name}" });

        return user;
    }

    public Household AddHousehold(User creator, string name = "Casa")
    {
        var household = new Household { Name = name, CreatedByUserId = creator.Id };
        Store.Households.Add(household);
        creator.ActiveHouseholdId = household.Id;

        return household;
    }

    public Membership Join(User user, Household household, MembershipRole role)
    {
        _joinedAt = _joinedAt.AddMinutes(1);
        var membership = new Membership
        {
            UserId = user.Id,
            HouseholdId = household.Id,
            Household = household,
            Role = role,
            Status = MembershipStatus.Active,
            CreatedAt = _joinedAt
        };
        Store.Memberships.Add(membership);

        return membership;
    }
}

public class FakeAccountStore : IAccountStore
{
    public List<User> Users { get; } = [];

    public List<Household> Households { get; } = [];

    public List<Membership> Memberships { get; } = [];

    public List<UserCredential> Credentials { get; } = [];

    public List<ExternalLogin> ExternalLogins { get; } = [];

    public List<RefreshToken> RefreshTokens { get; } = [];

    public Task<User?> FindUserAsync(Guid userId, CancellationToken cancellationToken)
        => Task.FromResult(Users.FirstOrDefault(u => u.Id == userId));

    public Task<IReadOnlyList<Membership>> ListActiveMembershipsOfUserAsync(Guid userId, CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyList<Membership>>(
            Memberships.Where(m => m.UserId == userId && m.Status == MembershipStatus.Active).ToList());

    public Task<IReadOnlyList<Membership>> ListActiveMembersAsync(Guid householdId, CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyList<Membership>>(
            Memberships.Where(m => m.HouseholdId == householdId && m.Status == MembershipStatus.Active).OrderBy(m => m.CreatedAt).ToList());

    public Task RemoveSignInDataAsync(Guid userId, CancellationToken cancellationToken)
    {
        Credentials.RemoveAll(c => c.UserId == userId);
        ExternalLogins.RemoveAll(l => l.UserId == userId);
        RefreshTokens.RemoveAll(t => t.UserId == userId);

        return Task.CompletedTask;
    }

    public void RemoveHousehold(Household household)
    {
        Households.Remove(household);
        Memberships.RemoveAll(m => m.HouseholdId == household.Id);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
