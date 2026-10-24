using Application.Auth;

namespace Application.Abstractions;

public interface IExternalTokenValidator
{
    Task<ExternalIdentity?> ValidateAsync(SocialProvider provider, string idToken, CancellationToken cancellationToken);
}

public record ExternalIdentity(string Subject, string? Email, bool EmailVerified, string? Name);
