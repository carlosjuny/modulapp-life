using System.Security.Claims;
using API.Authorization;
using Application.EventsLog;
using Domain.Enums;

namespace API.Endpoints;

public static class ApiEndpoints
{
    public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGroup("/auth").WithTags("Auth").MapAuthEndpoints();
        app.MapGroup("/me").WithTags("Me").RequireAuthorization().MapAccountEndpoints();

        app.MapGroup("/invites").WithTags("Households").RequireAuthorization().MapInvitationEndpoints();

        var households = app.MapGroup("/households").WithTags("Households").RequireAuthorization();
        households.MapHouseholdEndpoints();
        households.MapGet("/{householdId:guid}/events-log", GetEventsLogAsync).RequireAuthorization(HouseholdAccessRequirement.AdminPolicy);

        return app;
    }

    private static async Task<IResult> GetEventsLogAsync(
        Guid householdId,
        ClaimsPrincipal user,
        EventLogService service,
        CancellationToken cancellationToken,
        EventLogType? type = null,
        int page = 1,
        int pageSize = 50)
    {
        if (!Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var requesterId))
        {
            return Results.Unauthorized();
        }

        var result = await service.QueryAsync(householdId, requesterId, type, page, pageSize, cancellationToken);
        if (!result.IsAllowed)
        {
            return Results.Problem(statusCode: StatusCodes.Status403Forbidden, title: "Solo el administrador del hogar puede consultar el registro.");
        }

        return Results.Ok(result.Items.Select(e => new
        {
            e.Id,
            e.HouseholdId,
            e.ActorUserId,
            Type = e.Type.ToString(),
            e.Payload,
            e.CreatedAt
        }));
    }
}
