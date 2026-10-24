using Application.Auth;
using Domain.Entities;

namespace Application.Tests.Auth;

public class AuthServiceRegistroLoginTests
{
    [Fact]
    public async Task Register_ConDatosValidos_CreaUsuarioConHashYDevuelveJwt()
    {
        var ctx = new AuthTestContext();

        var result = await ctx.Service.RegisterAsync("  Ana@Example.com ", "clave1234", "Ana");

        Assert.True(result.IsSuccess);
        Assert.Equal("ana@example.com", result.Tokens!.User.Email);
        Assert.False(string.IsNullOrEmpty(result.Tokens.AccessToken));
        Assert.Equal("hash:clave1234", Assert.Single(ctx.Store.Credentials).PasswordHash);
        Assert.DoesNotContain("clave1234", result.Tokens.AccessToken);
    }

    [Fact]
    public async Task Register_ConCorreoExistente_FallaConMensajeGenerico()
    {
        var ctx = new AuthTestContext();
        await ctx.Service.RegisterAsync("ana@example.com", "clave1234", "Ana");

        var result = await ctx.Service.RegisterAsync("ana@example.com", "otraClave99", "Otra Ana");

        Assert.Equal(AuthStatus.RegistrationFailed, result.Status);
        Assert.DoesNotContain("existe", result.Error!, StringComparison.OrdinalIgnoreCase);
        Assert.Single(ctx.Store.Users);
    }

    [Theory]
    [InlineData("corta1")]
    [InlineData("solo letras sin numeros")]
    [InlineData("1234567890")]
    public async Task Register_ConContrasenaDebil_FallaValidacion(string password)
    {
        var ctx = new AuthTestContext();

        var result = await ctx.Service.RegisterAsync("ana@example.com", password, "Ana");

        Assert.Equal(AuthStatus.ValidationFailed, result.Status);
        Assert.Empty(ctx.Store.Users);
    }

    [Fact]
    public async Task Register_ConContrasenaDeOchoCaracteresConLetrasYNumeros_LaAcepta()
    {
        var ctx = new AuthTestContext();

        var result = await ctx.Service.RegisterAsync("ana@example.com", "abcd1234", "Ana");

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Register_ConContrasenaDeSieteCaracteres_FallaValidacion()
    {
        var ctx = new AuthTestContext();

        var result = await ctx.Service.RegisterAsync("ana@example.com", "abcd123", "Ana");

        Assert.Equal(AuthStatus.ValidationFailed, result.Status);
    }

    [Theory]
    [InlineData("sin-arroba")]
    [InlineData("")]
    [InlineData(null)]
    public async Task Register_ConCorreoInvalido_FallaValidacion(string? email)
    {
        var ctx = new AuthTestContext();

        var result = await ctx.Service.RegisterAsync(email, "clave1234", "Ana");

        Assert.Equal(AuthStatus.ValidationFailed, result.Status);
        Assert.Empty(ctx.Store.Users);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task Register_SinNombre_FallaValidacion(string? name)
    {
        var ctx = new AuthTestContext();

        var result = await ctx.Service.RegisterAsync("ana@example.com", "clave1234", name);

        Assert.Equal(AuthStatus.ValidationFailed, result.Status);
        Assert.Empty(ctx.Store.Users);
    }

    [Fact]
    public async Task Login_ConCredencialesCorrectas_DevuelveJwt()
    {
        var ctx = new AuthTestContext();
        await ctx.Service.RegisterAsync("ana@example.com", "clave1234", "Ana");

        var result = await ctx.Service.LoginAsync("ANA@example.com", "clave1234");

        Assert.True(result.IsSuccess);
        Assert.Equal("Ana", result.Tokens!.User.Name);
        Assert.False(string.IsNullOrEmpty(result.Tokens.AccessToken));
    }

    [Fact]
    public async Task Login_ConContrasenaIncorrecta_FallaSinToken()
    {
        var ctx = new AuthTestContext();
        await ctx.Service.RegisterAsync("ana@example.com", "clave1234", "Ana");

        var result = await ctx.Service.LoginAsync("ana@example.com", "incorrecta1");

        Assert.Equal(AuthStatus.InvalidCredentials, result.Status);
        Assert.Null(result.Tokens);
    }

    [Fact]
    public async Task Login_ConCorreoInexistente_DevuelveElMismoErrorQueContrasenaIncorrecta()
    {
        var ctx = new AuthTestContext();
        await ctx.Service.RegisterAsync("ana@example.com", "clave1234", "Ana");

        var unknownUser = await ctx.Service.LoginAsync("nadie@example.com", "clave1234");
        var wrongPassword = await ctx.Service.LoginAsync("ana@example.com", "incorrecta1");

        Assert.Equal(wrongPassword.Status, unknownUser.Status);
        Assert.Equal(wrongPassword.Error, unknownUser.Error);
    }

    [Fact]
    public async Task Login_DeUsuarioSinCredencial_Falla()
    {
        var ctx = new AuthTestContext();
        ctx.Store.Add(new User { Email = "sin-clave@example.com", Name = "Sin clave" });

        var result = await ctx.Service.LoginAsync("sin-clave@example.com", "clave1234");

        Assert.Equal(AuthStatus.InvalidCredentials, result.Status);
    }
}
