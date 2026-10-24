using Application.Households;
using Domain.Enums;

namespace Application.Tests.Households;

public class HouseholdPermissionsServiceTests
{
    [Fact]
    public async Task ActivarModulo_ComoMiembro_EsProhibido()
    {
        var ctx = new HouseholdTestContext();
        var ana = ctx.AddUser("Ana");
        var luis = ctx.AddUser("Luis");
        var householdId = await ctx.CreateHouseholdAsync(ana);
        await ctx.JoinAsync(householdId, ana, luis);

        var result = await ctx.Service.SetModuleActiveAsync(luis.Id, householdId, "tasks", true);

        Assert.Equal(HouseholdStatus.Forbidden, result.Status);
        Assert.Empty(ctx.Store.Modules);
    }

    [Fact]
    public async Task ActivarModulo_ComoAdministrador_LoDejaActivo()
    {
        var ctx = new HouseholdTestContext();
        var ana = ctx.AddUser("Ana");
        var householdId = await ctx.CreateHouseholdAsync(ana);

        var result = await ctx.Service.SetModuleActiveAsync(ana.Id, householdId, " Tasks ", true);

        Assert.True(result.IsSuccess);
        Assert.Equal("tasks", ctx.Store.Modules.Single().ModuleKey);
        Assert.True(ctx.Store.Modules.Single().IsActive);
    }

    [Fact]
    public async Task CambiarPlan_ComoMiembro_EsProhibidoYNoCambiaElPlan()
    {
        var ctx = new HouseholdTestContext();
        var ana = ctx.AddUser("Ana");
        var luis = ctx.AddUser("Luis");
        var householdId = await ctx.CreateHouseholdAsync(ana);
        await ctx.JoinAsync(householdId, ana, luis);

        var result = await ctx.Service.ChangePlanAsync(luis.Id, householdId, "premium");

        Assert.Equal(HouseholdStatus.Forbidden, result.Status);
        Assert.Equal("free", ctx.Store.Households.Single().Plan);
    }

    [Fact]
    public async Task CambiarPlan_ComoAdministrador_ActualizaHogarYSuscripcion()
    {
        var ctx = new HouseholdTestContext();
        var ana = ctx.AddUser("Ana");
        var householdId = await ctx.CreateHouseholdAsync(ana);

        var result = await ctx.Service.ChangePlanAsync(ana.Id, householdId, "Premium");

        Assert.True(result.IsSuccess);
        Assert.Equal("premium", ctx.Store.Households.Single().Plan);
        Assert.Equal("premium", ctx.Store.Subscriptions.Single().Plan);
    }

    [Fact]
    public async Task CambiarRol_AscenderUnMiembro_LoVuelveAdministrador()
    {
        var ctx = new HouseholdTestContext();
        var ana = ctx.AddUser("Ana");
        var luis = ctx.AddUser("Luis");
        var householdId = await ctx.CreateHouseholdAsync(ana);
        await ctx.JoinAsync(householdId, ana, luis);

        var result = await ctx.Service.ChangeRoleAsync(ana.Id, householdId, luis.Id, MembershipRole.Admin);

        Assert.True(result.IsSuccess);
        Assert.Equal(MembershipRole.Admin, ctx.MembershipOf(luis, householdId).Role);
    }

    [Fact]
    public async Task CambiarRol_ComoMiembro_EsProhibido()
    {
        var ctx = new HouseholdTestContext();
        var ana = ctx.AddUser("Ana");
        var luis = ctx.AddUser("Luis");
        var householdId = await ctx.CreateHouseholdAsync(ana);
        await ctx.JoinAsync(householdId, ana, luis);

        var result = await ctx.Service.ChangeRoleAsync(luis.Id, householdId, luis.Id, MembershipRole.Admin);

        Assert.Equal(HouseholdStatus.Forbidden, result.Status);
        Assert.Equal(MembershipRole.Member, ctx.MembershipOf(luis, householdId).Role);
    }

    [Fact]
    public async Task CambiarRol_DegradarAlUnicoAdministrador_EsConflicto()
    {
        var ctx = new HouseholdTestContext();
        var ana = ctx.AddUser("Ana");
        var householdId = await ctx.CreateHouseholdAsync(ana);

        var result = await ctx.Service.ChangeRoleAsync(ana.Id, householdId, ana.Id, MembershipRole.Member);

        Assert.Equal(HouseholdStatus.Conflict, result.Status);
        Assert.Equal(MembershipRole.Admin, ctx.MembershipOf(ana, householdId).Role);
    }

    [Fact]
    public async Task CambiarRol_AOwner_FallaValidacion()
    {
        var ctx = new HouseholdTestContext();
        var ana = ctx.AddUser("Ana");
        var luis = ctx.AddUser("Luis");
        var householdId = await ctx.CreateHouseholdAsync(ana);
        await ctx.JoinAsync(householdId, ana, luis);

        var result = await ctx.Service.ChangeRoleAsync(ana.Id, householdId, luis.Id, MembershipRole.Owner);

        Assert.Equal(HouseholdStatus.ValidationFailed, result.Status);
    }
}
