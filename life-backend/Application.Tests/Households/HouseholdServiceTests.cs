using Application.Households;
using Domain.Enums;

namespace Application.Tests.Households;

public class HouseholdServiceTests
{
    [Fact]
    public async Task Create_ConNombreValido_DejaAlCreadorComoAdministradorYHogarActivo()
    {
        var ctx = new HouseholdTestContext();
        var ana = ctx.AddUser("Ana");

        var result = await ctx.Service.CreateAsync(ana.Id, "  Casa Ana ");

        Assert.True(result.IsSuccess);
        Assert.Equal("Casa Ana", result.Value!.Name);
        Assert.Equal(MembershipRole.Admin, ctx.MembershipOf(ana, result.Value.Id).Role);
        Assert.Equal(result.Value.Id, ana.ActiveHouseholdId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task Create_SinNombre_FallaValidacion(string? name)
    {
        var ctx = new HouseholdTestContext();
        var ana = ctx.AddUser("Ana");

        var result = await ctx.Service.CreateAsync(ana.Id, name);

        Assert.Equal(HouseholdStatus.ValidationFailed, result.Status);
        Assert.Empty(ctx.Store.Households);
    }

    [Fact]
    public async Task AceptarInvitacion_TresUsuarios_FormanUnHogarDeTresMiembros()
    {
        var ctx = new HouseholdTestContext();
        var ana = ctx.AddUser("Ana");
        var luis = ctx.AddUser("Luis");
        var eva = ctx.AddUser("Eva");
        var householdId = await ctx.CreateHouseholdAsync(ana);

        await ctx.JoinAsync(householdId, ana, luis);
        await ctx.JoinAsync(householdId, ana, eva);
        var members = await ctx.Service.ListMembersAsync(ana.Id, householdId);

        Assert.Equal(3, members.Value!.Count);
        Assert.Equal(MembershipRole.Member, ctx.MembershipOf(luis, householdId).Role);
    }

    [Fact]
    public async Task AceptarInvitacion_ValidaPorPrimeraVez_DevuelveElNombreDelHogar()
    {
        var ctx = new HouseholdTestContext();
        var ana = ctx.AddUser("Ana");
        var luis = ctx.AddUser("Luis");
        var householdId = await ctx.CreateHouseholdAsync(ana);
        var invitation = await ctx.Service.CreateInvitationAsync(ana.Id, householdId);

        var result = await ctx.Service.AcceptInvitationAsync(luis.Id, invitation.Value!.Code);

        Assert.True(result.IsSuccess);
        Assert.Equal("Casa", result.Value!.Name);
        Assert.Equal(householdId, luis.ActiveHouseholdId);
    }

    [Fact]
    public async Task AceptarInvitacion_Expirada_EsRechazada()
    {
        var ctx = new HouseholdTestContext();
        var ana = ctx.AddUser("Ana");
        var luis = ctx.AddUser("Luis");
        var householdId = await ctx.CreateHouseholdAsync(ana);
        var invitation = await ctx.Service.CreateInvitationAsync(ana.Id, householdId);
        ctx.Clock.Advance(HouseholdService.InvitationLifetime + TimeSpan.FromMinutes(1));

        var result = await ctx.Service.AcceptInvitationAsync(luis.Id, invitation.Value!.Code);

        Assert.Equal(HouseholdStatus.InvalidInvitation, result.Status);
        Assert.DoesNotContain(ctx.Store.Memberships, m => m.UserId == luis.Id);
    }

    [Fact]
    public async Task AceptarInvitacion_YaUsada_EsRechazada()
    {
        var ctx = new HouseholdTestContext();
        var ana = ctx.AddUser("Ana");
        var luis = ctx.AddUser("Luis");
        var eva = ctx.AddUser("Eva");
        var householdId = await ctx.CreateHouseholdAsync(ana);
        var invitation = await ctx.Service.CreateInvitationAsync(ana.Id, householdId);
        await ctx.Service.AcceptInvitationAsync(luis.Id, invitation.Value!.Code);

        var result = await ctx.Service.AcceptInvitationAsync(eva.Id, invitation.Value.Code);

        Assert.Equal(HouseholdStatus.InvalidInvitation, result.Status);
        Assert.DoesNotContain(ctx.Store.Memberships, m => m.UserId == eva.Id);
    }

    [Fact]
    public async Task AceptarInvitacion_ConCodigoInexistente_EsRechazada()
    {
        var ctx = new HouseholdTestContext();
        var luis = ctx.AddUser("Luis");

        var result = await ctx.Service.AcceptInvitationAsync(luis.Id, "no-existe");

        Assert.Equal(HouseholdStatus.InvalidInvitation, result.Status);
    }

    [Fact]
    public async Task CrearInvitacion_ComoMiembro_EsProhibido()
    {
        var ctx = new HouseholdTestContext();
        var ana = ctx.AddUser("Ana");
        var luis = ctx.AddUser("Luis");
        var householdId = await ctx.CreateHouseholdAsync(ana);
        await ctx.JoinAsync(householdId, ana, luis);

        var result = await ctx.Service.CreateInvitationAsync(luis.Id, householdId);

        Assert.Equal(HouseholdStatus.Forbidden, result.Status);
    }

    [Fact]
    public async Task Salir_ElUltimoAdministrador_EsRechazadoYSigueSiendoMiembro()
    {
        var ctx = new HouseholdTestContext();
        var ana = ctx.AddUser("Ana");
        var luis = ctx.AddUser("Luis");
        var householdId = await ctx.CreateHouseholdAsync(ana);
        await ctx.JoinAsync(householdId, ana, luis);

        var result = await ctx.Service.LeaveAsync(ana.Id, householdId);

        Assert.Equal(HouseholdStatus.Conflict, result.Status);
        Assert.Equal(MembershipStatus.Active, ctx.MembershipOf(ana, householdId).Status);
    }

    [Fact]
    public async Task Salir_ElAdministradorTrasTransferir_LoPermite()
    {
        var ctx = new HouseholdTestContext();
        var ana = ctx.AddUser("Ana");
        var luis = ctx.AddUser("Luis");
        var householdId = await ctx.CreateHouseholdAsync(ana);
        await ctx.JoinAsync(householdId, ana, luis);
        await ctx.Service.TransferAdminAsync(ana.Id, householdId, luis.Id);

        var result = await ctx.Service.LeaveAsync(ana.Id, householdId);

        Assert.True(result.IsSuccess);
        Assert.Equal(MembershipRole.Admin, ctx.MembershipOf(luis, householdId).Role);
        Assert.Equal(MembershipStatus.Removed, ctx.MembershipOf(ana, householdId).Status);
    }

    [Fact]
    public async Task Salir_ComoMiembro_LoPermiteYLimpiaElHogarActivo()
    {
        var ctx = new HouseholdTestContext();
        var ana = ctx.AddUser("Ana");
        var luis = ctx.AddUser("Luis");
        var householdId = await ctx.CreateHouseholdAsync(ana);
        await ctx.JoinAsync(householdId, ana, luis);

        var result = await ctx.Service.LeaveAsync(luis.Id, householdId);

        Assert.True(result.IsSuccess);
        Assert.Equal(MembershipStatus.Removed, ctx.MembershipOf(luis, householdId).Status);
        Assert.Null(luis.ActiveHouseholdId);
    }

    [Fact]
    public async Task Expulsar_ComoAdministrador_QuitaAlMiembroYLeNiegaElAcceso()
    {
        var ctx = new HouseholdTestContext();
        var ana = ctx.AddUser("Ana");
        var luis = ctx.AddUser("Luis");
        var householdId = await ctx.CreateHouseholdAsync(ana);
        await ctx.JoinAsync(householdId, ana, luis);

        var result = await ctx.Service.RemoveMemberAsync(ana.Id, householdId, luis.Id);
        var access = await ctx.Service.ListMembersAsync(luis.Id, householdId);

        Assert.True(result.IsSuccess);
        Assert.Equal(HouseholdStatus.Forbidden, access.Status);
    }

    [Fact]
    public async Task Expulsar_ComoMiembro_EsProhibido()
    {
        var ctx = new HouseholdTestContext();
        var ana = ctx.AddUser("Ana");
        var luis = ctx.AddUser("Luis");
        var eva = ctx.AddUser("Eva");
        var householdId = await ctx.CreateHouseholdAsync(ana);
        await ctx.JoinAsync(householdId, ana, luis);
        await ctx.JoinAsync(householdId, ana, eva);

        var result = await ctx.Service.RemoveMemberAsync(luis.Id, householdId, eva.Id);

        Assert.Equal(HouseholdStatus.Forbidden, result.Status);
        Assert.Equal(MembershipStatus.Active, ctx.MembershipOf(eva, householdId).Status);
    }

    [Fact]
    public async Task CambiarHogarActivo_ConVariosHogares_ActualizaElActivo()
    {
        var ctx = new HouseholdTestContext();
        var ana = ctx.AddUser("Ana");
        var luis = ctx.AddUser("Luis");
        var casaAna = await ctx.CreateHouseholdAsync(ana);
        var casaLuis = await ctx.CreateHouseholdAsync(luis);
        await ctx.JoinAsync(casaAna, ana, luis);
        await ctx.Service.SetActiveAsync(luis.Id, casaLuis);

        var result = await ctx.Service.SetActiveAsync(luis.Id, casaAna);

        Assert.True(result.IsSuccess);
        Assert.Equal(casaAna, luis.ActiveHouseholdId);
    }

    [Fact]
    public async Task CambiarHogarActivo_DeUnHogarAjeno_EsProhibido()
    {
        var ctx = new HouseholdTestContext();
        var ana = ctx.AddUser("Ana");
        var luis = ctx.AddUser("Luis");
        var casaAna = await ctx.CreateHouseholdAsync(ana);

        var result = await ctx.Service.SetActiveAsync(luis.Id, casaAna);

        Assert.Equal(HouseholdStatus.Forbidden, result.Status);
        Assert.Null(luis.ActiveHouseholdId);
    }
}
