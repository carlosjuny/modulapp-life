using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class ModuleActivationConfiguration : IEntityTypeConfiguration<ModuleActivation>
{
    public void Configure(EntityTypeBuilder<ModuleActivation> builder)
    {
        builder.ToTable("module_activations");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.ModuleKey).HasMaxLength(50).IsRequired();
        builder.Property(m => m.CreatedAt).IsRequired();

        builder.HasOne(m => m.Household)
            .WithMany(h => h.ModuleActivations)
            .HasForeignKey(m => m.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(m => new { m.HouseholdId, m.ModuleKey }).IsUnique();
    }
}
