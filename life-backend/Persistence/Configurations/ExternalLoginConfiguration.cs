using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class ExternalLoginConfiguration : IEntityTypeConfiguration<ExternalLogin>
{
    public void Configure(EntityTypeBuilder<ExternalLogin> builder)
    {
        builder.ToTable("external_logins");
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Provider).HasMaxLength(20).IsRequired();
        builder.Property(l => l.Subject).HasMaxLength(255).IsRequired();
        builder.Property(l => l.CreatedAt).IsRequired();

        builder.HasOne(l => l.User)
            .WithMany()
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(l => new { l.Provider, l.Subject }).IsUnique();
        builder.HasIndex(l => l.UserId);
    }
}
