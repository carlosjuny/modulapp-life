using Application.Auth;

namespace API.Endpoints;

public static class AuthEndpoints
{
    public record RegisterRequest(string? Email, string? Password, string? Name);

    public record LoginRequest(string? Email, string? Password);

    public record RefreshRequest(string? RefreshToken);

    public static RouteGroupBuilder MapAuthEndpoints(this RouteGroupBuilder auth)
    {
        auth.MapPost("/register", RegisterAsync);
        auth.MapPost("/login", LoginAsync);
        auth.MapPost("/refresh", RefreshAsync);
        auth.MapPost("/logout", LogoutAsync);

        return auth;
    }

    private static async Task<IResult> RegisterAsync(RegisterRequest request, AuthService service, CancellationToken cancellationToken)
        => ToResult(await service.RegisterAsync(request.Email, request.Password, request.Name, cancellationToken));

    private static async Task<IResult> LoginAsync(LoginRequest request, AuthService service, CancellationToken cancellationToken)
        => ToResult(await service.LoginAsync(request.Email, request.Password, cancellationToken));

    private static async Task<IResult> RefreshAsync(RefreshRequest request, AuthService service, CancellationToken cancellationToken)
        => ToResult(await service.RefreshAsync(request.RefreshToken, cancellationToken));

    private static async Task<IResult> LogoutAsync(RefreshRequest request, AuthService service, CancellationToken cancellationToken)
    {
        await service.LogoutAsync(request.RefreshToken, cancellationToken);

        return Results.NoContent();
    }

    private static IResult ToResult(AuthResult result)
    {
        if (result.IsSuccess)
        {
            return Results.Ok(result.Tokens);
        }

        var statusCode = result.Status is AuthStatus.InvalidCredentials or AuthStatus.InvalidRefreshToken
            ? StatusCodes.Status401Unauthorized
            : StatusCodes.Status400BadRequest;

        return Results.Problem(statusCode: statusCode, title: result.Error);
    }
}
