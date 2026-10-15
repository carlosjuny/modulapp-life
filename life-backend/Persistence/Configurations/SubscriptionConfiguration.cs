using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("subscriptions");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Plan).HasMaxLength(50).IsRequired();
        builder.Property(s => s.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(s => s.Provider).HasMaxLength(50);
        builder.Property(s => s.CreatedAt).IsRequired();

        builder.HasOne(s => s.Household)
            .WithOne(h => h.Subscription)
            .HasForeignKey<Subscription>(s => s.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => s.HouseholdId).IsUnique();
    }
}
