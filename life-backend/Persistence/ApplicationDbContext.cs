using Application.Abstractions;
using Domain.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentHousehold? currentHousehold = null) : DbContext(options)
{
    // Sin hogar en la solicitud el filtro no devuelve filas (falla cerrado).
    private Guid? CurrentHouseholdId => currentHousehold?.Id;

    public DbSet<User> Users => Set<User>();

    public DbSet<Household> Households => Set<Household>();

    public DbSet<Membership> Memberships => Set<Membership>();

    public DbSet<ModuleActivation> ModuleActivations => Set<ModuleActivation>();

    public DbSet<Subscription> Subscriptions => Set<Subscription>();

    public DbSet<EventLog> EventsLog => Set<EventLog>();

    public DbSet<UserCredential> UserCredentials => Set<UserCredential>();

    public DbSet<Invitation> Invitations => Set<Invitation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Membership e Invitation quedan fuera: se consultan entre hogares (mis hogares, aceptar invitación por código)
        // y siempre se acotan con ForHousehold o por pertenencia del usuario.
        modelBuilder.Entity<ModuleActivation>().HasQueryFilter(e => e.HouseholdId == CurrentHouseholdId);
        modelBuilder.Entity<Subscription>().HasQueryFilter(e => e.HouseholdId == CurrentHouseholdId);
        modelBuilder.Entity<EventLog>().HasQueryFilter(e => e.HouseholdId == CurrentHouseholdId);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
