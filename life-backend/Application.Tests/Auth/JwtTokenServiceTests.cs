using System.Security.Claims;
using System.Text;
using Domain.Entities;
using Infrastructure.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Application.Tests.Auth;

public class JwtTokenServiceTests
{
    private static readonly JwtOptions Options = new()
    {
        Issuer = "modulapp-life",
        Audience = "modulapp-life-app",
        SigningKey = "clave-de-prueba-de-al-menos-32-caracteres",
        AccessTokenMinutes = 15
    };

    private static TokenValidationParameters ValidationParameters(string signingKey) => new()
    {
        ValidIssuer = Options.Issuer,
        ValidAudience = Options.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    [Fact]
    public async Task CreateAccessToken_ConUsuarioValido_GeneraJwtFirmadoConSusClaims()
    {
        var clock = new FakeClock();
        var service = new JwtTokenService(Microsoft.Extensions.Options.Options.Create(Options), clock);
        var user = new User { Email = "ana@example.com", Name = "Ana" };

        var token = service.CreateAccessToken(user);

        var validation = await new JsonWebTokenHandler().ValidateTokenAsync(token.Value, ValidationParameters(Options.SigningKey));
        Assert.True(validation.IsValid);
        Assert.Equal(user.Id.ToString(), validation.ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        Assert.Equal(clock.Now.UtcDateTime.AddMinutes(15), token.ExpiresAt);
    }

    [Fact]
    public void CreateAccessToken_ConOpcionesPorDefecto_VenceALos15Minutos()
    {
        var clock = new FakeClock();
        var options = new JwtOptions { Issuer = Options.Issuer, Audience = Options.Audience, SigningKey = Options.SigningKey };
        var service = new JwtTokenService(Microsoft.Extensions.Options.Options.Create(options), clock);

        var token = service.CreateAccessToken(new User { Email = "ana@example.com", Name = "Ana" });

        Assert.Equal(clock.Now.UtcDateTime.AddMinutes(15), token.ExpiresAt);
    }

    [Fact]
    public async Task CreateAccessToken_ValidadoConOtraClave_EsRechazado()
    {
        var service = new JwtTokenService(Microsoft.Extensions.Options.Options.Create(Options), new FakeClock());
        var token = service.CreateAccessToken(new User { Email = "ana@example.com", Name = "Ana" });

        var validation = await new JsonWebTokenHandler().ValidateTokenAsync(token.Value, ValidationParameters("otra-clave-distinta-de-al-menos-32-caracteres"));

        Assert.False(validation.IsValid);
    }

    [Fact]
    public async Task CreateAccessToken_ConTokenVencido_EsRechazado()
    {
        var clock = new FakeClock { Now = DateTimeOffset.UtcNow.AddHours(-1) };
        var service = new JwtTokenService(Microsoft.Extensions.Options.Options.Create(Options), clock);
        var token = service.CreateAccessToken(new User { Email = "ana@example.com", Name = "Ana" });

        var validation = await new JsonWebTokenHandler().ValidateTokenAsync(token.Value, ValidationParameters(Options.SigningKey));

        Assert.False(validation.IsValid);
    }
}

public class FakeClock : TimeProvider
{
    public DateTimeOffset Now { get; set; } = DateTimeOffset.UtcNow;

    public override DateTimeOffset GetUtcNow() => Now;
}
