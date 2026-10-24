namespace Application.Auth;

public enum AuthStatus
{
    Success,
    ValidationFailed,
    RegistrationFailed,
    InvalidCredentials,
    InvalidRefreshToken,
    InvalidSocialToken
}

public record AuthUser(Guid Id, string Email, string Name);

public record AuthTokens(string AccessToken, DateTime AccessTokenExpiresAt, AuthUser User, string RefreshToken, DateTime RefreshTokenExpiresAt);

public record AuthResult(AuthStatus Status, AuthTokens? Tokens = null, string? Error = null)
{
    public bool IsSuccess => Status == AuthStatus.Success;

    public static AuthResult Success(AuthTokens tokens) => new(AuthStatus.Success, tokens);

    public static AuthResult Fail(AuthStatus status, string error) => new(status, null, error);
}
