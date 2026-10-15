using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public static class DevelopmentDataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.Users.AnyAsync(cancellationToken))
        {
            return;
        }

        var ana = new User { Email = "ana@example.com", Name = "Ana Prueba" };
        var luis = new User { Email = "luis@example.com", Name = "Luis Prueba" };

        var household = new Household
        {
            Name = "Hogar de prueba",
            Plan = "free",
            TimeZone = "America/Bogota",
            CreatedByUser = ana
        };

        household.Memberships.Add(new Membership { User = ana, Role = MembershipRole.Owner, Status = MembershipStatus.Active });
        household.Memberships.Add(new Membership { User = luis, Role = MembershipRole.Member, Status = MembershipStatus.Active });

        var activeModules = new[] { "tasks", "lists" };
        foreach (var key in new[] { "tasks", "lists", "agenda", "notes" })
        {
            var isActive = activeModules.Contains(key);
            household.ModuleActivations.Add(new ModuleActivation
            {
                ModuleKey = key,
                IsActive = isActive,
                ActivatedAt = isActive ? DateTime.UtcNow : null
            });
        }

        household.Subscription = new Subscription { Plan = "free", Status = SubscriptionStatus.Trial };

        db.Users.AddRange(ana, luis);
        db.Households.Add(household);
        await db.SaveChangesAsync(cancellationToken);
    }
}
