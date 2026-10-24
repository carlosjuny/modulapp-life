using Application.Auth;

namespace Application.Tests.Auth;

public class AuthServiceRefreshTokenTests
{
    private static async Task<(AuthTestContext Ctx, AuthTokens Tokens)> RegisteredAsync()
    {
        var ctx = new AuthTestContext();
        var result = await ctx.Service.RegisterAsync("ana@example.com", "clave1234", "Ana");

        return (ctx, result.Tokens!);
    }

    [Fact]
    public async Task Register_ConDatosValidos_DevuelveRefreshTokenGuardadoSoloComoHash()
    {
        var ctx = new AuthTestContext();

        var result = await ctx.Service.RegisterAsync("ana@example.com", "clave1234", "Ana");

        var stored = Assert.Single(ctx.Store.RefreshTokens);
        Assert.False(string.IsNullOrEmpty(result.Tokens!.RefreshToken));
        Assert.NotEqual(result.Tokens.RefreshToken, stored.TokenHash);
        Assert.Equal(ctx.Clock.Now.UtcDateTime.AddDays(30), result.Tokens.RefreshTokenExpiresAt);
    }

    [Fact]
    public async Task Login_ConCredencialesCorrectas_EmiteUnRefreshTokenNuevo()
    {
        var (ctx, registered) = await RegisteredAsync();

        var result = await ctx.Service.LoginAsync("ana@example.com", "clave1234");

        Assert.True(result.IsSuccess);
        Assert.NotEqual(registered.RefreshToken, result.Tokens!.RefreshToken);
        Assert.Equal(2, ctx.Store.RefreshTokens.Count);
    }

    [Fact]
    public async Task Refresh_ConTokenValido_EntregaNuevosTokensYRotaElAnterior()
    {
        var (ctx, registered) = await RegisteredAsync();

        var result = await ctx.Service.RefreshAsync(registered.RefreshToken);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(registered.RefreshToken, result.Tokens!.RefreshToken);
        var old = ctx.Store.RefreshTokens[0];
        Assert.NotNull(old.RevokedAt);
        Assert.Equal(ctx.Store.RefreshTokens[1].Id, old.ReplacedByTokenId);
        Assert.Null(ctx.Store.RefreshTokens[1].RevokedAt);
    }

    [Fact]
    public async Task Refresh_ConTokenYaRotado_LoRechazaYRevocaLasSesionesActivasDelUsuario()
    {
        var (ctx, registered) = await RegisteredAsync();
        var renewed = await ctx.Service.RefreshAsync(registered.RefreshToken);

        var reuse = await ctx.Service.RefreshAsync(registered.RefreshToken);

        Assert.Equal(AuthStatus.InvalidRefreshToken, reuse.Status);
        Assert.All(ctx.Store.RefreshTokens, t => Assert.NotNull(t.RevokedAt));
        Assert.Equal(AuthStatus.InvalidRefreshToken, (await ctx.Service.RefreshAsync(renewed.Tokens!.RefreshToken)).Status);
    }

    [Fact]
    public async Task Refresh_ConTokenRevocadoPorLogout_LoRechaza()
    {
        var (ctx, registered) = await RegisteredAsync();
        await ctx.Service.LogoutAsync(registered.RefreshToken);

        var result = await ctx.Service.RefreshAsync(registered.RefreshToken);

        Assert.Equal(AuthStatus.InvalidRefreshToken, result.Status);
    }

    [Fact]
    public async Task Refresh_ConTokenVencido_LoRechaza()
    {
        var (ctx, registered) = await RegisteredAsync();
        ctx.Clock.Now = ctx.Clock.Now.AddDays(31);

        var result = await ctx.Service.RefreshAsync(registered.RefreshToken);

        Assert.Equal(AuthStatus.InvalidRefreshToken, result.Status);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("token-inexistente")]
    public async Task Refresh_ConTokenAusenteODesconocido_LoRechazaConElMismoMensajeQueUnoVencido(string? token)
    {
        var (ctx, registered) = await RegisteredAsync();
        ctx.Clock.Now = ctx.Clock.Now.AddDays(31);
        var expired = await ctx.Service.RefreshAsync(registered.RefreshToken);

        var result = await ctx.Service.RefreshAsync(token);

        Assert.Equal(AuthStatus.InvalidRefreshToken, result.Status);
        Assert.Equal(expired.Error, result.Error);
    }

    [Fact]
    public async Task Refresh_ConRotacionConcurrente_RechazaLaSolicitudQuePierde()
    {
        var (ctx, registered) = await RegisteredAsync();
        ctx.Store.ConcurrentRotationDetected = true;

        var result = await ctx.Service.RefreshAsync(registered.RefreshToken);

        Assert.Equal(AuthStatus.InvalidRefreshToken, result.Status);
    }

    [Fact]
    public async Task Logout_ConTokenActivo_LoRevoca()
    {
        var (ctx, registered) = await RegisteredAsync();

        await ctx.Service.LogoutAsync(registered.RefreshToken);

        Assert.NotNull(Assert.Single(ctx.Store.RefreshTokens).RevokedAt);
    }

    [Fact]
    public async Task Logout_ConTokenDesconocido_NoFallaNiRevocaOtrasSesiones()
    {
        var (ctx, _) = await RegisteredAsync();

        await ctx.Service.LogoutAsync("token-inexistente");

        Assert.Null(Assert.Single(ctx.Store.RefreshTokens).RevokedAt);
    }
}
