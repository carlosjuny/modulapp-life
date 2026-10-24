using System.Security.Cryptography;
using Application.Auth;
using Infrastructure.Auth;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Application.Tests.Auth;

public class SocialTokenValidatorTests
{
    private const string GoogleClientId = "google-client";
    private const string AppleClientId = "apple-client";
    private const string GoogleIssuer = "https://accounts.google.com";
    private const string AppleIssuer = "https://appleid.apple.com";

    private static readonly RsaSecurityKey SigningKey = new(RSA.Create(2048)) { KeyId = "k1" };
    private static readonly RsaSecurityKey OtherKey = new(RSA.Create(2048)) { KeyId = "k1" };

    private static SocialTokenValidator CreateValidator(FakeClock clock, string[]? googleIds = null)
    {
        var options = Microsoft.Extensions.Options.Options.Create(new SocialAuthOptions
        {
            Google = new SocialAuthOptions.ProviderOptions { ClientIds = googleIds ?? [GoogleClientId] },
            Apple = new SocialAuthOptions.ProviderOptions { ClientIds = [AppleClientId] }
        });

        return new SocialTokenValidator(options, new FixedKeyProvider(SigningKey), clock);
    }

    private static string CreateToken(FakeClock clock, string issuer, string audience, RsaSecurityKey? key = null, TimeSpan? lifetime = null, object? emailVerified = null)
    {
        var now = clock.Now.UtcDateTime;
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Audience = audience,
            NotBefore = now.AddMinutes(-5),
            IssuedAt = now.AddMinutes(-5),
            Expires = now.Add(lifetime ?? TimeSpan.FromMinutes(10)),
            SigningCredentials = new SigningCredentials(key ?? SigningKey, SecurityAlgorithms.RsaSha256),
            Claims = new Dictionary<string, object>
            {
                ["sub"] = "provider-sub-1",
                ["email"] = "ana@example.com",
                ["email_verified"] = emailVerified ?? true,
                ["name"] = "Ana"
            }
        };

        return new JsonWebTokenHandler().CreateToken(descriptor);
    }

    [Fact]
    public async Task Validate_ConTokenDeGoogleValido_DevuelveLaIdentidad()
    {
        var clock = new FakeClock();
        var token = CreateToken(clock, GoogleIssuer, GoogleClientId);

        var identity = await CreateValidator(clock).ValidateAsync(SocialProvider.Google, token, CancellationToken.None);

        Assert.NotNull(identity);
        Assert.Equal("provider-sub-1", identity.Subject);
        Assert.Equal("ana@example.com", identity.Email);
        Assert.True(identity.EmailVerified);
    }

    [Fact]
    public async Task Validate_ConTokenDeAppleValidoConCorreoVerificadoComoTexto_DevuelveLaIdentidadVerificada()
    {
        var clock = new FakeClock();
        var token = CreateToken(clock, AppleIssuer, AppleClientId, emailVerified: "true");

        var identity = await CreateValidator(clock).ValidateAsync(SocialProvider.Apple, token, CancellationToken.None);

        Assert.NotNull(identity);
        Assert.True(identity.EmailVerified);
    }

    [Fact]
    public async Task Validate_ConFirmaDeOtraClave_LoRechaza()
    {
        var clock = new FakeClock();
        var token = CreateToken(clock, GoogleIssuer, GoogleClientId, key: OtherKey);

        var identity = await CreateValidator(clock).ValidateAsync(SocialProvider.Google, token, CancellationToken.None);

        Assert.Null(identity);
    }

    [Fact]
    public async Task Validate_ConAudienciaDistinta_LoRechaza()
    {
        var clock = new FakeClock();
        var token = CreateToken(clock, GoogleIssuer, "otra-app");

        var identity = await CreateValidator(clock).ValidateAsync(SocialProvider.Google, token, CancellationToken.None);

        Assert.Null(identity);
    }

    [Fact]
    public async Task Validate_ConEmisorDeOtroProveedor_LoRechaza()
    {
        var clock = new FakeClock();
        var token = CreateToken(clock, AppleIssuer, GoogleClientId);

        var identity = await CreateValidator(clock).ValidateAsync(SocialProvider.Google, token, CancellationToken.None);

        Assert.Null(identity);
    }

    [Fact]
    public async Task Validate_ConTokenExpirado_LoRechaza()
    {
        var clock = new FakeClock();
        var token = CreateToken(clock, GoogleIssuer, GoogleClientId, lifetime: TimeSpan.FromMinutes(1));
        clock.Now += TimeSpan.FromMinutes(2);

        var identity = await CreateValidator(clock).ValidateAsync(SocialProvider.Google, token, CancellationToken.None);

        Assert.Null(identity);
    }

    [Fact]
    public async Task Validate_SinClientIdsConfigurados_RechazaTodoToken()
    {
        var clock = new FakeClock();
        var token = CreateToken(clock, GoogleIssuer, GoogleClientId);

        var identity = await CreateValidator(clock, googleIds: []).ValidateAsync(SocialProvider.Google, token, CancellationToken.None);

        Assert.Null(identity);
    }

    [Fact]
    public async Task Validate_ConTextoQueNoEsUnJwt_LoRechaza()
    {
        var clock = new FakeClock();

        var identity = await CreateValidator(clock).ValidateAsync(SocialProvider.Google, "no-es-un-jwt", CancellationToken.None);

        Assert.Null(identity);
    }

    private class FixedKeyProvider(SecurityKey key) : ISocialSigningKeyProvider
    {
        public Task<IReadOnlyCollection<SecurityKey>> GetKeysAsync(SocialProvider provider, CancellationToken cancellationToken)
            => Task.FromResult<IReadOnlyCollection<SecurityKey>>([key]);
    }
}
