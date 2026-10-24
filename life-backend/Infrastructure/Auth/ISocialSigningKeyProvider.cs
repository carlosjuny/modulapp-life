using Application.Auth;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Auth;

public interface ISocialSigningKeyProvider
{
    Task<IReadOnlyCollection<SecurityKey>> GetKeysAsync(SocialProvider provider, CancellationToken cancellationToken);
}
