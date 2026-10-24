using Domain.Entities;

namespace Application.Abstractions;

public interface IAccountStore
{
    Task<User?> FindUserAsync(Guid userId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Membership>> ListActiveMembershipsOfUserAsync(Guid userId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Membership>> ListActiveMembersAsync(Guid householdId, CancellationToken cancellationToken);

    Task RemoveSignInDataAsync(Guid userId, CancellationToken cancellationToken);

    void RemoveHousehold(Household household);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
