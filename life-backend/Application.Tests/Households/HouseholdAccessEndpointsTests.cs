using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Application.Tests.Auth;

namespace Application.Tests.Households;

public class HouseholdAccessEndpointsTests : IClassFixture<AuthApiFactory>
{
    private readonly AuthApiFactory _factory;

    public HouseholdAccessEndpointsTests(AuthApiFactory factory)
    {
        _factory = factory;
    }

    private async Task<HttpClient> NewUserAsync(string name)
    {
        var client = _factory.CreateClient();
        var register = await client.PostAsJsonAsync("/auth/register", new { email = $"{Guid.NewGuid():N}@example.com", password = "clave1234", name });
        var token = (await register.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("accessToken").GetString();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private static async Task<Guid> CreateHouseholdAsync(HttpClient admin)
    {
        var response = await admin.PostAsJsonAsync("/households", new { name = "Casa" });
        return (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetGuid();
    }

    private static async Task<Guid> JoinAsync(HttpClient admin, Guid householdId, HttpClient member)
    {
        var invite = await admin.PostAsync($"/households/{householdId}/invites", null);
        var code = (await invite.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("code").GetString();
        await member.PostAsync($"/invites/{code}/accept", null);
        var members = await admin.GetFromJsonAsync<JsonElement>($"/households/{householdId}/members");
        return members.EnumerateArray().First(m => m.GetProperty("role").GetString() == "Member").GetProperty("userId").GetGuid();
    }

    [Fact]
    public async Task UsuarioAjenoAlHogar_EnCualquierRutaDelHogar_RecibeForbidden()
    {
        var ana = await NewUserAsync("Ana");
        var intruso = await NewUserAsync("Intruso");
        var householdId = await CreateHouseholdAsync(ana);

        var members = await intruso.GetAsync($"/households/{householdId}/members");
        var invites = await intruso.PostAsync($"/households/{householdId}/invites", null);
        var modules = await intruso.GetAsync($"/households/{householdId}/modules");
        var plan = await intruso.PutAsJsonAsync($"/households/{householdId}/subscription", new { plan = "premium" });
        var events = await intruso.GetAsync($"/households/{householdId}/events-log");

        Assert.Equal(HttpStatusCode.Forbidden, members.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, invites.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, modules.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, plan.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, events.StatusCode);
    }

    [Fact]
    public async Task Miembro_NoPuedeInvitarActivarModulosNiCambiarPlan_PeroSiConsultar()
    {
        var ana = await NewUserAsync("Ana");
        var luis = await NewUserAsync("Luis");
        var householdId = await CreateHouseholdAsync(ana);
        await JoinAsync(ana, householdId, luis);

        var invite = await luis.PostAsync($"/households/{householdId}/invites", null);
        var module = await luis.PutAsJsonAsync($"/households/{householdId}/modules/tasks", new { isActive = true });
        var plan = await luis.PutAsJsonAsync($"/households/{householdId}/subscription", new { plan = "premium" });
        var members = await luis.GetAsync($"/households/{householdId}/members");
        var modules = await luis.GetAsync($"/households/{householdId}/modules");

        Assert.Equal(HttpStatusCode.Forbidden, invite.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, module.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, plan.StatusCode);
        Assert.Equal(HttpStatusCode.OK, members.StatusCode);
        Assert.Equal(HttpStatusCode.OK, modules.StatusCode);
    }

    [Fact]
    public async Task Administrador_ActivaModulosCambiaPlanYCambiaRoles()
    {
        var ana = await NewUserAsync("Ana");
        var luis = await NewUserAsync("Luis");
        var householdId = await CreateHouseholdAsync(ana);
        var luisId = await JoinAsync(ana, householdId, luis);

        var module = await ana.PutAsJsonAsync($"/households/{householdId}/modules/tasks", new { isActive = true });
        var plan = await ana.PutAsJsonAsync($"/households/{householdId}/subscription", new { plan = "premium" });
        var role = await ana.PutAsJsonAsync($"/households/{householdId}/members/{luisId}/role", new { role = "Admin" });
        var modules = await ana.GetFromJsonAsync<JsonElement>($"/households/{householdId}/modules");

        Assert.Equal(HttpStatusCode.OK, module.StatusCode);
        Assert.Equal(HttpStatusCode.OK, plan.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, role.StatusCode);
        Assert.Equal("tasks", modules[0].GetProperty("moduleKey").GetString());
    }

    [Fact]
    public async Task ModulosDeUnHogar_NoSeVenDesdeOtroHogar()
    {
        var ana = await NewUserAsync("Ana");
        var eva = await NewUserAsync("Eva");
        var casaAna = await CreateHouseholdAsync(ana);
        var casaEva = await CreateHouseholdAsync(eva);
        await ana.PutAsJsonAsync($"/households/{casaAna}/modules/tasks", new { isActive = true });

        var propios = await eva.GetFromJsonAsync<JsonElement>($"/households/{casaEva}/modules");
        var ajenos = await eva.GetAsync($"/households/{casaAna}/modules");

        Assert.Equal(0, propios.GetArrayLength());
        Assert.Equal(HttpStatusCode.Forbidden, ajenos.StatusCode);
    }
}
