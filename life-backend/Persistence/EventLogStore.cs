using Application.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class EventLogStore(ApplicationDbContext db) : IEventLogStore
{
    public async Task AddAsync(EventLog entry, CancellationToken cancellationToken)
    {
        db.EventsLog.Add(entry);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EventLog>> ListByHouseholdAsync(
        Guid householdId, EventLogType? type, int skip, int take, CancellationToken cancellationToken)
    {
        var query = db.EventsLog.AsNoTracking().ForHousehold(householdId);

        if (type is not null)
        {
            query = query.Where(e => e.Type == type);
        }

        return await query
            .OrderByDescending(e => e.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<MembershipRole?> GetActiveRoleAsync(Guid userId, Guid householdId, CancellationToken cancellationToken)
        => await db.Memberships
            .AsNoTracking()
            .ForHousehold(householdId)
            .Where(m => m.UserId == userId && m.Status == MembershipStatus.Active)
            .Select(m => (MembershipRole?)m.Role)
            .FirstOrDefaultAsync(cancellationToken);
}
