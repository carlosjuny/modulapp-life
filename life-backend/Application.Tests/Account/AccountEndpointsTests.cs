using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Application.Tests.Auth;

namespace Application.Tests.Account;

public class AccountEndpointsTests : IClassFixture<AuthApiFactory>
{
    private const string Password = "clave1234";

    private readonly AuthApiFactory _factory;

    public AccountEndpointsTests(AuthApiFactory factory)
    {
        _factory = factory;
    }

    private async Task<(HttpClient Client, string Email, string RefreshToken)> NewUserAsync()
    {
        var client = _factory.CreateClient();
        var email = $"{Guid.NewGuid():N}@example.com";
        var register = await client.PostAsJsonAsync("/auth/register", new { email, password = Password, name = "Ana" });
        var body = await register.Content.ReadFromJsonAsync<JsonElement>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", body.GetProperty("accessToken").GetString());

        return (client, email, body.GetProperty("refreshToken").GetString()!);
    }

    [Fact]
    public async Task DeleteMe_SinToken_RecibeUnauthorized()
    {
        var anonymous = _factory.CreateClient();

        var response = await anonymous.DeleteAsync("/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteMe_UsuarioConHogar_NoPuedeIniciarSesionNiRenovarLaSesion()
    {
        var (client, email, refreshToken) = await NewUserAsync();
        await client.PostAsJsonAsync("/households", new { name = "Casa" });

        var deleted = await client.DeleteAsync("/me");
        var login = await _factory.CreateClient().PostAsJsonAsync("/auth/login", new { email, password = Password });
        var refresh = await _factory.CreateClient().PostAsJsonAsync("/auth/refresh", new { refreshToken });

        Assert.Equal(HttpStatusCode.NoContent, deleted.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, login.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, refresh.StatusCode);
    }

    [Fact]
    public async Task DeleteMe_TrasElBorrado_ElCorreoQuedaLibreParaRegistrarse()
    {
        var (client, email, _) = await NewUserAsync();
        await client.DeleteAsync("/me");

        var register = await _factory.CreateClient().PostAsJsonAsync("/auth/register", new { email, password = Password, name = "Ana" });

        Assert.Equal(HttpStatusCode.OK, register.StatusCode);
    }
}
