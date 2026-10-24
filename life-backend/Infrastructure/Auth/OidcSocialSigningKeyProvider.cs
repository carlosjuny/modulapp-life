using Application.Auth;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Auth;

public class OidcSocialSigningKeyProvider : ISocialSigningKeyProvider
{
    private readonly Dictionary<SocialProvider, ConfigurationManager<OpenIdConnectConfiguration>> _managers = new()
    {
        [SocialProvider.Google] = CreateManager("https://accounts.google.com/.well-known/openid-configuration"),
        [SocialProvider.Apple] = CreateManager("https://appleid.apple.com/.well-known/openid-configuration")
    };

    public async Task<IReadOnlyCollection<SecurityKey>> GetKeysAsync(SocialProvider provider, CancellationToken cancellationToken)
    {
        var configuration = await _managers[provider].GetConfigurationAsync(cancellationToken);

        return [.. configuration.SigningKeys];
    }

    private static ConfigurationManager<OpenIdConnectConfiguration> CreateManager(string metadataAddress)
        => new(metadataAddress, new OpenIdConnectConfigurationRetriever(), new HttpDocumentRetriever { RequireHttps = true });
}
