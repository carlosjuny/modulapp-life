using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Application.Tests.Auth;

public class AuthEndpointsTests : IClassFixture<AuthApiFactory>
{
    private readonly HttpClient _client;

    public AuthEndpointsTests(AuthApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    private static string NewEmail() => $"{Guid.NewGuid():N}@example.com";

    private static async Task<JsonElement> ReadAsync(HttpResponseMessage response)
        => await response.Content.ReadFromJsonAsync<JsonElement>();

    [Fact]
    public async Task Register_ConDatosValidos_DevuelveJwtSinContrasena()
    {
        var body = new { email = NewEmail(), password = "clave1234", name = "Ana" };

        var response = await _client.PostAsJsonAsync("/auth/register", body);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var raw = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("clave1234", raw);
        Assert.Equal(3, (await ReadAsync(response)).GetProperty("accessToken").GetString()!.Split('.').Length);
    }

    [Fact]
    public async Task Register_ConCorreoRepetido_Devuelve400Generico()
    {
        var email = NewEmail();
        await _client.PostAsJsonAsync("/auth/register", new { email, password = "clave1234", name = "Ana" });

        var response = await _client.PostAsJsonAsync("/auth/register", new { email, password = "otraClave99", name = "Otra" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.DoesNotContain("existe", await response.Content.ReadAsStringAsync(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Login_ConCredencialesCorrectas_DevuelveJwt()
    {
        var email = NewEmail();
        await _client.PostAsJsonAsync("/auth/register", new { email, password = "clave1234", name = "Ana" });

        var response = await _client.PostAsJsonAsync("/auth/login", new { email, password = "clave1234" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.False(string.IsNullOrEmpty((await ReadAsync(response)).GetProperty("accessToken").GetString()));
    }

    [Fact]
    public async Task Login_ConContrasenaIncorrectaOCorreoInexistente_Devuelve401ConElMismoMensaje()
    {
        var email = NewEmail();
        await _client.PostAsJsonAsync("/auth/register", new { email, password = "clave1234", name = "Ana" });

        var wrongPassword = await _client.PostAsJsonAsync("/auth/login", new { email, password = "incorrecta1" });
        var unknownEmail = await _client.PostAsJsonAsync("/auth/login", new { email = NewEmail(), password = "clave1234" });

        Assert.Equal(HttpStatusCode.Unauthorized, wrongPassword.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, unknownEmail.StatusCode);
        Assert.Equal(
            (await ReadAsync(wrongPassword)).GetProperty("title").GetString(),
            (await ReadAsync(unknownEmail)).GetProperty("title").GetString());
    }

    [Fact]
    public async Task EventsLog_SinTokenDevuelve401YConTokenYaNoDevuelve401()
    {
        var email = NewEmail();
        var register = await _client.PostAsJsonAsync("/auth/register", new { email, password = "clave1234", name = "Ana" });
        var token = (await ReadAsync(register)).GetProperty("accessToken").GetString();
        var url = $"/households/{Guid.NewGuid()}/events-log";

        var withoutToken = await _client.GetAsync(url);
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var withToken = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, withoutToken.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, withToken.StatusCode);
    }

    [Fact]
    public async Task Refresh_ConTokenValido_RenuevaYElTokenAnteriorQuedaRechazado()
    {
        var register = await _client.PostAsJsonAsync("/auth/register", new { email = NewEmail(), password = "clave1234", name = "Ana" });
        var refreshToken = (await ReadAsync(register)).GetProperty("refreshToken").GetString();

        var renewed = await _client.PostAsJsonAsync("/auth/refresh", new { refreshToken });
        var reused = await _client.PostAsJsonAsync("/auth/refresh", new { refreshToken });

        Assert.Equal(HttpStatusCode.OK, renewed.StatusCode);
        var body = await ReadAsync(renewed);
        Assert.False(string.IsNullOrEmpty(body.GetProperty("accessToken").GetString()));
        Assert.NotEqual(refreshToken, body.GetProperty("refreshToken").GetString());
        Assert.Equal(HttpStatusCode.Unauthorized, reused.StatusCode);
    }

    [Fact]
    public async Task Logout_ConRefreshToken_LoRevocaYDevuelve204()
    {
        var register = await _client.PostAsJsonAsync("/auth/register", new { email = NewEmail(), password = "clave1234", name = "Ana" });
        var refreshToken = (await ReadAsync(register)).GetProperty("refreshToken").GetString();

        var logout = await _client.PostAsJsonAsync("/auth/logout", new { refreshToken });
        var refresh = await _client.PostAsJsonAsync("/auth/refresh", new { refreshToken });

        Assert.Equal(HttpStatusCode.NoContent, logout.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, refresh.StatusCode);
    }

    [Fact]
    public async Task Refresh_ConTokenInexistente_Devuelve401SinRevelarElMotivo()
    {
        var response = await _client.PostAsJsonAsync("/auth/refresh", new { refreshToken = "token-inexistente" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.DoesNotContain("revoc", await response.Content.ReadAsStringAsync(), StringComparison.OrdinalIgnoreCase);
    }
}
