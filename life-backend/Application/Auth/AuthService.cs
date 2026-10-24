using Application.Abstractions;
using Domain.Entities;

namespace Application.Auth;

public class AuthService(IAuthStore store, IPasswordHasher passwordHasher, ITokenService tokenService)
{
    private const string InvalidCredentialsMessage = "Correo o contraseña incorrectos.";
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
        await store.SaveChangesAsync(cancellationToken);

        return AuthResult.Success(IssueToken(user));
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

        return AuthResult.Success(IssueToken(user));
    }

    private AuthTokens IssueToken(User user)
    {
        var access = tokenService.CreateAccessToken(user);

        return new AuthTokens(access.Value, access.ExpiresAt, new AuthUser(user.Id, user.Email, user.Name));
    }
}
