using Application.Account;
using Domain.Enums;

namespace Application.Tests.Account;

public class AccountServiceTests
{
    [Fact]
    public async Task Delete_UsuarioConDatos_AnonimizaLosDatosPersonales()
    {
        var ctx = new AccountTestContext();
        var ana = ctx.AddUser("Ana");

        var status = await ctx.Service.DeleteAsync(ana.Id);

        Assert.Equal(AccountStatus.Success, status);
        Assert.Equal(AccountService.AnonymizedEmail(ana.Id), ana.Email);
        Assert.Equal(AccountService.DeletedUserName, ana.Name);
        Assert.Null(ana.AvatarUrl);
    }

    [Fact]
    public async Task Delete_UsuarioConDatos_EliminaCredencialesLoginsExternosYRefreshTokens()
    {
        var ctx = new AccountTestContext();
        var ana = ctx.AddUser("Ana");
        var luis = ctx.AddUser("Luis");

        await ctx.Service.DeleteAsync(ana.Id);

        Assert.DoesNotContain(ctx.Store.Credentials, c => c.UserId == ana.Id);
        Assert.DoesNotContain(ctx.Store.ExternalLogins, l => l.UserId == ana.Id);
        Assert.DoesNotContain(ctx.Store.RefreshTokens, t => t.UserId == ana.Id);
        Assert.Single(ctx.Store.Credentials, c => c.UserId == luis.Id);
    }

    [Fact]
    public async Task Delete_UsuarioInexistente_DevuelveNotFound()
    {
        var ctx = new AccountTestContext();

        var status = await ctx.Service.DeleteAsync(Guid.NewGuid());

        Assert.Equal(AccountStatus.NotFound, status);
    }

    [Fact]
    public async Task Delete_UnicoAdminConOtrosMiembros_TransfiereLaAdministracionAlMiembroMasAntiguo()
    {
        var ctx = new AccountTestContext();
        var ana = ctx.AddUser("Ana");
        var luis = ctx.AddUser("Luis");
        var eva = ctx.AddUser("Eva");
        var household = ctx.AddHousehold(ana);
        var anaMembership = ctx.Join(ana, household, MembershipRole.Admin);
        var luisMembership = ctx.Join(luis, household, MembershipRole.Member);
        var evaMembership = ctx.Join(eva, household, MembershipRole.Member);

        await ctx.Service.DeleteAsync(ana.Id);

        Assert.Contains(household, ctx.Store.Households);
        Assert.Equal(MembershipStatus.Removed, anaMembership.Status);
        Assert.Equal(MembershipRole.Admin, luisMembership.Role);
        Assert.Equal(MembershipRole.Member, evaMembership.Role);
        Assert.Null(ana.ActiveHouseholdId);
    }

    [Fact]
    public async Task Delete_UnicoMiembroDelHogar_EliminaElHogar()
    {
        var ctx = new AccountTestContext();
        var ana = ctx.AddUser("Ana");
        var household = ctx.AddHousehold(ana);
        ctx.Join(ana, household, MembershipRole.Admin);

        await ctx.Service.DeleteAsync(ana.Id);

        Assert.Empty(ctx.Store.Households);
        Assert.Empty(ctx.Store.Memberships);
    }

    [Fact]
    public async Task Delete_AdminConOtroAdministrador_NoCambiaLosRolesDelHogar()
    {
        var ctx = new AccountTestContext();
        var ana = ctx.AddUser("Ana");
        var luis = ctx.AddUser("Luis");
        var eva = ctx.AddUser("Eva");
        var household = ctx.AddHousehold(ana);
        ctx.Join(ana, household, MembershipRole.Admin);
        var luisMembership = ctx.Join(luis, household, MembershipRole.Admin);
        var evaMembership = ctx.Join(eva, household, MembershipRole.Member);

        await ctx.Service.DeleteAsync(ana.Id);

        Assert.Equal(MembershipRole.Admin, luisMembership.Role);
        Assert.Equal(MembershipRole.Member, evaMembership.Role);
        Assert.Contains(household, ctx.Store.Households);
    }

    [Fact]
    public async Task Delete_MiembroSinPermisosDeAdministrador_SoloSaleDelHogar()
    {
        var ctx = new AccountTestContext();
        var ana = ctx.AddUser("Ana");
        var luis = ctx.AddUser("Luis");
        var household = ctx.AddHousehold(ana);
        var anaMembership = ctx.Join(ana, household, MembershipRole.Admin);
        var luisMembership = ctx.Join(luis, household, MembershipRole.Member);

        await ctx.Service.DeleteAsync(luis.Id);

        Assert.Equal(MembershipStatus.Removed, luisMembership.Status);
        Assert.Equal(MembershipStatus.Active, anaMembership.Status);
        Assert.Equal(MembershipRole.Admin, anaMembership.Role);
    }
}
