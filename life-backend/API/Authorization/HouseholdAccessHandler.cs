using System.Security.Claims;
using Application.Abstractions;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace API.Authorization;

public class HouseholdAccessHandler(IHouseholdStore store) : AuthorizationHandler<HouseholdAccessRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, HouseholdAccessRequirement requirement)
    {
        if (context.Resource is not HttpContext http
            || !Guid.TryParse(http.GetRouteValue("householdId")?.ToString(), out var householdId)
            || !Guid.TryParse(context.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            return;
        }

        var membership = await store.FindMembershipAsync(userId, householdId, http.RequestAborted);
        if (membership is not { Status: MembershipStatus.Active })
        {
            return;
        }

        if (!requirement.AdminOnly || membership.Role is MembershipRole.Admin or MembershipRole.Owner)
        {
            context.Succeed(requirement);
        }
    }
}
