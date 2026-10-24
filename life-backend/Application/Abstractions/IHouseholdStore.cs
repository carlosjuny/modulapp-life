using Domain.Entities;

namespace Application.Abstractions;

public interface IHouseholdStore
{
    Task<User?> FindUserAsync(Guid userId, CancellationToken cancellationToken);

    Task<Membership?> FindMembershipAsync(Guid userId, Guid householdId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Membership>> ListActiveMembershipsOfUserAsync(Guid userId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Membership>> ListActiveMembersAsync(Guid householdId, CancellationToken cancellationToken);

    Task<int> CountActiveAdminsAsync(Guid householdId, CancellationToken cancellationToken);

    Task<Invitation?> FindInvitationAsync(string code, CancellationToken cancellationToken);

    void Add<TEntity>(TEntity entity) where TEntity : class;

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
