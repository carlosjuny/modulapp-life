using System.Security.Claims;
using Application.Households;

namespace API.Endpoints;

public static class HouseholdEndpoints
{
    public record CreateHouseholdRequest(string? Name);

    public record SetActiveHouseholdRequest(Guid HouseholdId);

    public record TransferAdminRequest(Guid UserId);

    public static RouteGroupBuilder MapHouseholdEndpoints(this RouteGroupBuilder households)
    {
        households.MapPost("/", CreateAsync);
        households.MapGet("/", ListMineAsync);
        households.MapPut("/active", SetActiveAsync);
        households.MapPost("/{householdId:guid}/invites", CreateInvitationAsync);
        households.MapGet("/{householdId:guid}/members", ListMembersAsync);
        households.MapDelete("/{householdId:guid}/members/{userId:guid}", RemoveMemberAsync);
        households.MapPost("/{householdId:guid}/leave", LeaveAsync);
        households.MapPost("/{householdId:guid}/transfer-admin", TransferAdminAsync);

        return households;
    }

    public static RouteGroupBuilder MapInvitationEndpoints(this RouteGroupBuilder invites)
    {
        invites.MapPost("/{code}/accept", AcceptAsync);

        return invites;
    }

    private static Task<IResult> CreateAsync(CreateHouseholdRequest request, ClaimsPrincipal user, HouseholdService service, CancellationToken cancellationToken)
        => WithUser(user, async id =>
        {
            var result = await service.CreateAsync(id, request.Name, cancellationToken);
            return result.IsSuccess
                ? Results.Created($"/households/{result.Value!.Id}", result.Value)
                : ToProblem(result);
        });

    private static Task<IResult> ListMineAsync(ClaimsPrincipal user, HouseholdService service, CancellationToken cancellationToken)
        => WithUser(user, async id => ToResult(await service.ListMineAsync(id, cancellationToken)));

    private static Task<IResult> SetActiveAsync(SetActiveHouseholdRequest request, ClaimsPrincipal user, HouseholdService service, CancellationToken cancellationToken)
        => WithUser(user, async id => ToNoContent(await service.SetActiveAsync(id, request.HouseholdId, cancellationToken)));

    private static Task<IResult> CreateInvitationAsync(Guid householdId, ClaimsPrincipal user, HouseholdService service, CancellationToken cancellationToken)
        => WithUser(user, async id => ToResult(await service.CreateInvitationAsync(id, householdId, cancellationToken)));

    private static Task<IResult> AcceptAsync(string code, ClaimsPrincipal user, HouseholdService service, CancellationToken cancellationToken)
        => WithUser(user, async id => ToResult(await service.AcceptInvitationAsync(id, code, cancellationToken)));

    private static Task<IResult> ListMembersAsync(Guid householdId, ClaimsPrincipal user, HouseholdService service, CancellationToken cancellationToken)
        => WithUser(user, async id => ToResult(await service.ListMembersAsync(id, householdId, cancellationToken)));

    private static Task<IResult> RemoveMemberAsync(Guid householdId, Guid userId, ClaimsPrincipal user, HouseholdService service, CancellationToken cancellationToken)
        => WithUser(user, async id => ToNoContent(await service.RemoveMemberAsync(id, householdId, userId, cancellationToken)));

    private static Task<IResult> LeaveAsync(Guid householdId, ClaimsPrincipal user, HouseholdService service, CancellationToken cancellationToken)
        => WithUser(user, async id => ToNoContent(await service.LeaveAsync(id, householdId, cancellationToken)));

    private static Task<IResult> TransferAdminAsync(Guid householdId, TransferAdminRequest request, ClaimsPrincipal user, HouseholdService service, CancellationToken cancellationToken)
        => WithUser(user, async id => ToNoContent(await service.TransferAdminAsync(id, householdId, request.UserId, cancellationToken)));

    private static Task<IResult> WithUser(ClaimsPrincipal user, Func<Guid, Task<IResult>> action)
        => Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)
            ? action(userId)
            : Task.FromResult(Results.Unauthorized());

    private static IResult ToResult<T>(HouseholdResult<T> result)
        => result.IsSuccess ? Results.Ok(result.Value) : ToProblem(result);

    private static IResult ToNoContent(HouseholdResult<Unit> result)
        => result.IsSuccess ? Results.NoContent() : ToProblem(result);

    private static IResult ToProblem<T>(HouseholdResult<T> result)
    {
        var statusCode = result.Status switch
        {
            HouseholdStatus.Forbidden => StatusCodes.Status403Forbidden,
            HouseholdStatus.NotFound => StatusCodes.Status404NotFound,
            HouseholdStatus.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status400BadRequest
        };

        return Results.Problem(statusCode: statusCode, title: result.Error);
    }
}
