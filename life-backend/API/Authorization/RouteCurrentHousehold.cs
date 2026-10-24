using Application.Abstractions;

namespace API.Authorization;

public class RouteCurrentHousehold(IHttpContextAccessor accessor) : ICurrentHousehold
{
    public Guid? Id => Guid.TryParse(accessor.HttpContext?.GetRouteValue("householdId")?.ToString(), out var id) ? id : null;
}
