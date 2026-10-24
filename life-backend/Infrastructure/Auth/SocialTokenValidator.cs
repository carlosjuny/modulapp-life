using Application.Abstractions;
using Application.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Auth;

public class SocialTokenValidator(IOptions<SocialAuthOptions> options, ISocialSigningKeyProvider keyProvider, TimeProvider clock) : IExternalTokenValidator
{
    private static readonly string[] GoogleIssuers = ["https://accounts.google.com", "accounts.google.com"];
    private const string AppleIssuer = "https://appleid.apple.com";

    private readonly SocialAuthOptions _options = options.Value;

    public async Task<ExternalIdentity?> ValidateAsync(SocialProvider provider, string idToken, CancellationToken cancellationToken)
    {
        var clientIds = (provider == SocialProvider.Google ? _options.Google.ClientIds : _options.Apple.ClientIds)
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .ToArray();
        if (clientIds.Length == 0)
        {
            return null;
        }

        IReadOnlyCollection<SecurityKey> keys;
        try
        {
            keys = await keyProvider.GetKeysAsync(provider, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return null;
        }

        var parameters = new TokenValidationParameters
        {
            ValidIssuers = provider == SocialProvider.Google ? GoogleIssuers : [AppleIssuer],
            ValidAudiences = clientIds,
            IssuerSigningKeys = keys,
            ValidAlgorithms = [SecurityAlgorithms.RsaSha256],
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            RequireSignedTokens = true,
            RequireExpirationTime = true,
            ValidateLifetime = true,
            LifetimeValidator = (notBefore, expires, _, _) => IsCurrent(notBefore, expires),
            ClockSkew = TimeSpan.Zero
        };

        var result = await new JsonWebTokenHandler().ValidateTokenAsync(idToken, parameters);
        if (!result.IsValid || result.SecurityToken is not JsonWebToken token)
        {
            return null;
        }

        token.TryGetPayloadValue<string>("email", out var email);
        token.TryGetPayloadValue<string>("name", out var name);

        return new ExternalIdentity(token.Subject, email, IsEmailVerified(token), name);
    }

    private bool IsCurrent(DateTime? notBefore, DateTime? expires)
    {
        var now = clock.GetUtcNow().UtcDateTime;

        return expires is not null && expires > now && (notBefore is null || notBefore <= now);
    }

    private static bool IsEmailVerified(JsonWebToken token)
    {
        if (token.TryGetPayloadValue<bool>("email_verified", out var asBool))
        {
            return asBool;
        }

        return token.TryGetPayloadValue<string>("email_verified", out var asString)
            && string.Equals(asString, "true", StringComparison.OrdinalIgnoreCase);
    }
}
