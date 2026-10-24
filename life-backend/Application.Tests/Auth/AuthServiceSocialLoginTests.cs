using Application.Abstractions;
using Application.Auth;
using Domain.Entities;

namespace Application.Tests.Auth;

public class AuthServiceSocialLoginTests
{
    private static ExternalIdentity Identity(string subject = "sub-1", string? email = "ana@example.com", bool verified = true, string? name = "Ana")
        => new(subject, email, verified, name);

    [Theory]
    [InlineData(SocialProvider.Google, "google")]
    [InlineData(SocialProvider.Apple, "Apple")]
    public async Task SocialLogin_ConTokenValidoDeCadaProveedor_CreaElUsuarioYDevuelveJwtYRefreshToken(SocialProvider provider, string providerName)
    {
        var ctx = new AuthTestContext();
        ctx.ExternalValidator.ValidTokens[(provider, "token-ok")] = Identity();

        var result = await ctx.Service.SocialLoginAsync(providerName, "token-ok");

        Assert.True(result.IsSuccess);
        Assert.False(string.IsNullOrEmpty(result.Tokens!.AccessToken));
        Assert.False(string.IsNullOrEmpty(result.Tokens.RefreshToken));
        var user = Assert.Single(ctx.Store.Users);
        Assert.Equal("ana@example.com", user.Email);
        Assert.Equal(provider.ToString(), Assert.Single(ctx.Store.ExternalLogins).Provider);
        Assert.Empty(ctx.Store.Credentials);
    }

    [Fact]
    public async Task SocialLogin_ConTokenInvalido_LoRechazaYNoCreaNada()
    {
        var ctx = new AuthTestContext();

        var result = await ctx.Service.SocialLoginAsync("google", "token-falso");

        Assert.Equal(AuthStatus.InvalidSocialToken, result.Status);
        Assert.Empty(ctx.Store.Users);
        Assert.Empty(ctx.Store.RefreshTokens);
    }

    [Fact]
    public async Task SocialLogin_ConTokenDeOtroProveedor_LoRechaza()
    {
        var ctx = new AuthTestContext();
        ctx.ExternalValidator.ValidTokens[(SocialProvider.Google, "token-google")] = Identity();

        var result = await ctx.Service.SocialLoginAsync("apple", "token-google");

        Assert.Equal(AuthStatus.InvalidSocialToken, result.Status);
        Assert.Empty(ctx.Store.Users);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("facebook")]
    public async Task SocialLogin_ConProveedorNoSoportado_FallaLaValidacion(string? provider)
    {
        var ctx = new AuthTestContext();

        var result = await ctx.Service.SocialLoginAsync(provider, "token-ok");

        Assert.Equal(AuthStatus.ValidationFailed, result.Status);
    }

    [Fact]
    public async Task SocialLogin_ConTokenVacio_LoRechaza()
    {
        var ctx = new AuthTestContext();

        var result = await ctx.Service.SocialLoginAsync("google", "  ");

        Assert.Equal(AuthStatus.InvalidSocialToken, result.Status);
    }

    [Fact]
    public async Task SocialLogin_ConCorreoVerificadoDeUsuarioExistente_LoVinculaAUnaUnicaCuenta()
    {
        var ctx = new AuthTestContext();
        await ctx.Service.RegisterAsync("ana@example.com", "clave1234", "Ana");
        ctx.ExternalValidator.ValidTokens[(SocialProvider.Google, "token-google")] = Identity("g-1");
        ctx.ExternalValidator.ValidTokens[(SocialProvider.Apple, "token-apple")] = Identity("a-1");

        var google = await ctx.Service.SocialLoginAsync("google", "token-google");
        var apple = await ctx.Service.SocialLoginAsync("apple", "token-apple");

        Assert.True(google.IsSuccess);
        Assert.True(apple.IsSuccess);
        var user = Assert.Single(ctx.Store.Users);
        Assert.Equal(user.Id, google.Tokens!.User.Id);
        Assert.Equal(user.Id, apple.Tokens!.User.Id);
        Assert.Equal(2, ctx.Store.ExternalLogins.Count);
    }

    [Fact]
    public async Task SocialLogin_ConCuentaYaVinculada_IniciaSesionSinDuplicarElVinculo()
    {
        var ctx = new AuthTestContext();
        ctx.ExternalValidator.ValidTokens[(SocialProvider.Google, "token-ok")] = Identity();
        var first = await ctx.Service.SocialLoginAsync("google", "token-ok");

        var second = await ctx.Service.SocialLoginAsync("google", "token-ok");

        Assert.True(second.IsSuccess);
        Assert.Equal(first.Tokens!.User.Id, second.Tokens!.User.Id);
        Assert.Single(ctx.Store.Users);
        Assert.Single(ctx.Store.ExternalLogins);
        Assert.Equal(2, ctx.Store.RefreshTokens.Count);
    }

    [Fact]
    public async Task SocialLogin_ConCorreoNoVerificado_NoVinculaNiCreaLaCuenta()
    {
        var ctx = new AuthTestContext();
        await ctx.Service.RegisterAsync("ana@example.com", "clave1234", "Ana");
        ctx.ExternalValidator.ValidTokens[(SocialProvider.Google, "token-ok")] = Identity(verified: false);

        var result = await ctx.Service.SocialLoginAsync("google", "token-ok");

        Assert.Equal(AuthStatus.InvalidSocialToken, result.Status);
        Assert.Empty(ctx.Store.ExternalLogins);
        Assert.Single(ctx.Store.Users);
    }

    [Fact]
    public async Task SocialLogin_SinCorreoEnElToken_LoRechaza()
    {
        var ctx = new AuthTestContext();
        ctx.ExternalValidator.ValidTokens[(SocialProvider.Apple, "token-ok")] = Identity(email: null);

        var result = await ctx.Service.SocialLoginAsync("apple", "token-ok");

        Assert.Equal(AuthStatus.InvalidSocialToken, result.Status);
        Assert.Empty(ctx.Store.Users);
    }

    [Fact]
    public async Task SocialLogin_ConCorreoExistenteONo_DevuelveElMismoMensajeDeError()
    {
        var ctx = new AuthTestContext();
        await ctx.Service.RegisterAsync("ana@example.com", "clave1234", "Ana");
        ctx.ExternalValidator.ValidTokens[(SocialProvider.Google, "existente")] = Identity(email: "ana@example.com", verified: false);
        ctx.ExternalValidator.ValidTokens[(SocialProvider.Google, "nuevo")] = Identity(subject: "sub-2", email: "otra@example.com", verified: false);

        var existing = await ctx.Service.SocialLoginAsync("google", "existente");
        var unknown = await ctx.Service.SocialLoginAsync("google", "nuevo");

        Assert.Equal(existing.Error, unknown.Error);
        Assert.Equal(existing.Status, unknown.Status);
    }
}
