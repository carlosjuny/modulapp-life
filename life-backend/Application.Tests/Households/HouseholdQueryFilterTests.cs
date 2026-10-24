using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Tests.Households;

public class HouseholdQueryFilterTests
{
    private sealed class FakeCurrentHousehold : ICurrentHousehold
    {
        public Guid? Id { get; set; }
    }

    private static ApplicationDbContext NewContext(FakeCurrentHousehold current)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options, current);
    }

    private static async Task<Guid> SeedAsync(ApplicationDbContext db)
    {
        var casa = new Household { Name = "Casa" };
        var otra = new Household { Name = "Otra" };
        db.Households.AddRange(casa, otra);
        db.ModuleActivations.Add(new ModuleActivation { HouseholdId = casa.Id, ModuleKey = "tasks" });
        db.ModuleActivations.Add(new ModuleActivation { HouseholdId = otra.Id, ModuleKey = "lists" });
        db.Subscriptions.Add(new Subscription { HouseholdId = casa.Id });
        db.Subscriptions.Add(new Subscription { HouseholdId = otra.Id });
        await db.SaveChangesAsync();
        return casa.Id;
    }

    [Fact]
    public async Task ConsultaConHogarActivo_DevuelveSoloRegistrosDeEseHogar()
    {
        var current = new FakeCurrentHousehold();
        await using var db = NewContext(current);
        var casa = await SeedAsync(db);
        current.Id = casa;

        var modules = await db.ModuleActivations.ToListAsync();
        var subscriptions = await db.Subscriptions.ToListAsync();

        Assert.Equal("tasks", Assert.Single(modules).ModuleKey);
        Assert.Equal(casa, Assert.Single(subscriptions).HouseholdId);
    }

    [Fact]
    public async Task ConsultaSinHogarEnLaSolicitud_NoDevuelveRegistros()
    {
        var current = new FakeCurrentHousehold();
        await using var db = NewContext(current);
        await SeedAsync(db);
        current.Id = null;

        var modules = await db.ModuleActivations.ToListAsync();

        Assert.Empty(modules);
    }
}
