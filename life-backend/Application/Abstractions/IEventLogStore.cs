using Domain.Entities;
using Domain.Enums;

namespace Application.Abstractions;

public interface IEventLogStore
{
    Task AddAsync(EventLog entry, CancellationToken cancellationToken);

    Task<IReadOnlyList<EventLog>> ListByHouseholdAsync(
        Guid householdId, EventLogType? type, int skip, int take, CancellationToken cancellationToken);

    Task<MembershipRole?> GetActiveRoleAsync(Guid userId, Guid householdId, CancellationToken cancellationToken);
}
