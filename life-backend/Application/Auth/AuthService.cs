using Application.Abstractions;
using Domain.Entities;

namespace Application.Auth;

public class AuthService(IAuthStore store, IPasswordHasher passwordHasher, ITokenService tokenService, IExternalTokenValidator externalTokenValidator, TimeProvider clock)
{
    private const string InvalidCredentialsMessage = "Correo o contraseña incorrectos.";
    private const string InvalidRefreshTokenMessage = "La sesión no es válida.";
    private const string InvalidSocialTokenMessage = "No se pudo iniciar sesión con el proveedor.";
    private const string RegistrationFailedMessage = "No se pudo completar el registro.";

    public async Task<AuthResult> RegisterAsync(string? email, string? password, string? name, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = AuthValidator.NormalizeEmail(email);
        var trimmedName = (name ?? string.Empty).Trim();

        if (!AuthValidator.IsValidEmail(normalizedEmail))
        {
            return AuthResult.Fail(AuthStatus.ValidationFailed, "El correo no es válido.");
        }

        if (trimmedName.Length is 0 or > 150)
        {
            return AuthResult.Fail(AuthStatus.ValidationFailed, "El nombre es obligatorio y no puede superar 150 caracteres.");
        }

        var passwordError = AuthValidator.ValidatePassword(password);
        if (passwordError is not null)
        {
            return AuthResult.Fail(AuthStatus.ValidationFailed, passwordError);
        }

        if (await store.FindUserByEmailAsync(normalizedEmail, cancellationToken) is not null)
        {
            return AuthResult.Fail(AuthStatus.RegistrationFailed, RegistrationFailedMessage);
        }

        var user = new User { Email = normalizedEmail, Name = trimmedName };
        store.Add(user);
        store.Add(new UserCredential { User = user, PasswordHash = passwordHasher.Hash(password!) });
        var tokens = IssueTokens(user);
        await store.SaveChangesAsync(cancellationToken);

        return AuthResult.Success(tokens);
    }

    public async Task<AuthResult> LoginAsync(string? email, string? password, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = AuthValidator.NormalizeEmail(email);
        var user = string.IsNullOrEmpty(normalizedEmail) ? null : await store.FindUserByEmailAsync(normalizedEmail, cancellationToken);
        var credential = user is null ? null : await store.FindCredentialAsync(user.Id, cancellationToken);

        var hash = credential?.PasswordHash ?? passwordHasher.Hash(Guid.NewGuid().ToString("N"));
        var passwordMatches = !string.IsNullOrEmpty(password) && passwordHasher.Verify(hash, password);

        if (user is null || credential is null || !passwordMatches)
        {
            return AuthResult.Fail(AuthStatus.InvalidCredentials, InvalidCredentialsMessage);
        }

        var tokens = IssueTokens(user);
        await store.SaveChangesAsync(cancellationToken);

        return AuthResult.Success(tokens);
    }

    public async Task<AuthResult> SocialLoginAsync(string? provider, string? idToken, CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<SocialProvider>(provider?.Trim(), ignoreCase: true, out var socialProvider) || !Enum.IsDefined(socialProvider))
        {
            return AuthResult.Fail(AuthStatus.ValidationFailed, "El proveedor no es válido.");
        }

        if (string.IsNullOrWhiteSpace(idToken))
        {
            return InvalidSocialToken();
        }

        var identity = await externalTokenValidator.ValidateAsync(socialProvider, idToken, cancellationToken);
        if (identity is null || string.IsNullOrWhiteSpace(identity.Subject))
        {
            return InvalidSocialToken();
        }

        var providerName = socialProvider.ToString();
        var link = await store.FindExternalLoginAsync(providerName, identity.Subject, cancellationToken);
        var user = link is null ? null : await store.FindUserByIdAsync(link.UserId, cancellationToken);

        if (user is null)
        {
            var email = AuthValidator.NormalizeEmail(identity.Email);
            if (!identity.EmailVerified || !AuthValidator.IsValidEmail(email))
            {
                return InvalidSocialToken();
            }

            user = await store.FindUserByEmailAsync(email, cancellationToken);
            if (user is null)
            {
                user = new User { Email = email, Name = ResolveName(identity.Name, email) };
                store.Add(user);
            }

            store.Add(new ExternalLogin { User = user, Provider = providerName, Subject = identity.Subject });
        }

        var tokens = IssueTokens(user);
        await store.SaveChangesAsync(cancellationToken);

        return AuthResult.Success(tokens);
    }

    public async Task<AuthResult> RefreshAsync(string? refreshToken, CancellationToken cancellationToken = default)
    {
        var stored = await FindRefreshTokenAsync(refreshToken, cancellationToken);
        if (stored is null)
        {
            return InvalidRefreshToken();
        }

        var now = clock.GetUtcNow().UtcDateTime;

        if (stored.ReplacedByTokenId is not null)
        {
            await store.RevokeActiveRefreshTokensAsync(stored.UserId, now, cancellationToken);
            await store.SaveChangesAsync(cancellationToken);

            return InvalidRefreshToken();
        }

        if (stored.RevokedAt is not null || stored.ExpiresAt <= now)
        {
            return InvalidRefreshToken();
        }

        var user = await store.FindUserByIdAsync(stored.UserId, cancellationToken);
        if (user is null)
        {
            return InvalidRefreshToken();
        }

        var tokens = IssueTokens(user, out var replacement);
        stored.RevokedAt = now;
        stored.ReplacedByTokenId = replacement.Id;

        if (!await store.TrySaveChangesAsync(cancellationToken))
        {
            return InvalidRefreshToken();
        }

        return AuthResult.Success(tokens);
    }

    public async Task LogoutAsync(string? refreshToken, CancellationToken cancellationToken = default)
    {
        var stored = await FindRefreshTokenAsync(refreshToken, cancellationToken);
        if (stored is null || stored.RevokedAt is not null)
        {
            return;
        }

        stored.RevokedAt = clock.GetUtcNow().UtcDateTime;
        await store.TrySaveChangesAsync(cancellationToken);
    }

    private Task<RefreshToken?> FindRefreshTokenAsync(string? refreshToken, CancellationToken cancellationToken)
        => string.IsNullOrWhiteSpace(refreshToken)
            ? Task.FromResult<RefreshToken?>(null)
            : store.FindRefreshTokenAsync(tokenService.HashRefreshToken(refreshToken), cancellationToken);

    private static AuthResult InvalidSocialToken() => AuthResult.Fail(AuthStatus.InvalidSocialToken, InvalidSocialTokenMessage);

    private static string ResolveName(string? name, string email)
    {
        var resolved = string.IsNullOrWhiteSpace(name) ? email[..email.IndexOf('@')] : name.Trim();

        return resolved.Length > 150 ? resolved[..150] : resolved;
    }

    private static AuthResult InvalidRefreshToken() => AuthResult.Fail(AuthStatus.InvalidRefreshToken, InvalidRefreshTokenMessage);

    private AuthTokens IssueTokens(User user) => IssueTokens(user, out _);

    private AuthTokens IssueTokens(User user, out RefreshToken refreshEntity)
    {
        var access = tokenService.CreateAccessToken(user);
        var refresh = tokenService.CreateRefreshToken();

        refreshEntity = new RefreshToken { User = user, TokenHash = refresh.Hash, ExpiresAt = refresh.ExpiresAt };
        store.Add(refreshEntity);

        return new AuthTokens(access.Value, access.ExpiresAt, new AuthUser(user.Id, user.Email, user.Name), refresh.Value, refresh.ExpiresAt);
    }
}
