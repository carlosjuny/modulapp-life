using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class HouseholdConfiguration : IEntityTypeConfiguration<Household>
{
    public void Configure(EntityTypeBuilder<Household> builder)
    {
        builder.ToTable("households");
        builder.HasKey(h => h.Id);

        builder.Property(h => h.Name).HasMaxLength(150).IsRequired();
        builder.Property(h => h.Plan).HasMaxLength(50).IsRequired();
        builder.Property(h => h.TimeZone).HasMaxLength(100).IsRequired();
        builder.Property(h => h.CreatedAt).IsRequired();

        builder.HasOne(h => h.CreatedByUser)
            .WithMany()
            .HasForeignKey(h => h.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(h => h.CreatedByUserId);
    }
}
