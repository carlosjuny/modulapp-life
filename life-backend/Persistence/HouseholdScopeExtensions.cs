using Domain.Common;

namespace Persistence;

public static class HouseholdScopeExtensions
{
    public static IQueryable<T> ForHousehold<T>(this IQueryable<T> query, Guid householdId)
        where T : class, IHouseholdScoped
        => query.Where(e => e.HouseholdId == householdId);
}
