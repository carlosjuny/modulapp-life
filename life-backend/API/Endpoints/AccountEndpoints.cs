using System.Security.Claims;
using Application.Account;

namespace API.Endpoints;

public static class AccountEndpoints
{
    public static RouteGroupBuilder MapAccountEndpoints(this RouteGroupBuilder me)
    {
        me.MapDelete("/", DeleteAsync);

        return me;
    }

    private static async Task<IResult> DeleteAsync(ClaimsPrincipal user, AccountService service, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            return Results.Unauthorized();
        }

        return await service.DeleteAsync(userId, cancellationToken) == AccountStatus.Success
            ? Results.NoContent()
            : Results.Problem(statusCode: StatusCodes.Status404NotFound, title: "Usuario no encontrado.");
    }
}
