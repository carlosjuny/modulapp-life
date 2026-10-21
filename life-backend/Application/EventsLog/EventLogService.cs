using Application.Abstractions;
using Domain.Entities;
using Domain.Enums;

namespace Application.EventsLog;

public class EventLogService(IEventLogStore store)
{
    public const int MaxPageSize = 100;

    public Task RecordAsync(
        Guid householdId,
        Guid actorUserId,
        EventLogType type,
        IReadOnlyDictionary<string, string?>? payload,
        CancellationToken cancellationToken = default)
    {
        var entry = new EventLog
        {
            HouseholdId = householdId,
            ActorUserId = actorUserId,
            Type = type,
            Payload = EventPayloadSanitizer.ToSafeJson(payload),
            CreatedAt = DateTime.UtcNow
        };

        return store.AddAsync(entry, cancellationToken);
    }

    public async Task<EventLogQueryResult> QueryAsync(
        Guid householdId,
        Guid requesterUserId,
        EventLogType? type,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var role = await store.GetActiveRoleAsync(requesterUserId, householdId, cancellationToken);
        if (role is not (MembershipRole.Admin or MembershipRole.Owner))
        {
            return EventLogQueryResult.Forbidden();
        }

        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);
        page = Math.Max(page, 1);

        var items = await store.ListByHouseholdAsync(householdId, type, (page - 1) * pageSize, pageSize, cancellationToken);

        return EventLogQueryResult.Ok(items);
    }
}

public record EventLogQueryResult(bool IsAllowed, IReadOnlyList<EventLog> Items)
{
    public static EventLogQueryResult Forbidden() => new(false, []);

    public static EventLogQueryResult Ok(IReadOnlyList<EventLog> items) => new(true, items);
}
