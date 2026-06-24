using EnterpriseIDP.Domain.Entities.Catalog;
using EnterpriseIDP.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseIDP.Infrastructure.Persistence.Configurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("Services");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
        builder.Property(s => s.Type).HasConversion<string>().HasMaxLength(50);
        builder.Property(s => s.Status).HasConversion<string>().HasMaxLength(50);

        builder.Property(s => s.Slug)
            .HasConversion(s => s.Value, v => ServiceSlug.Create(v).Value)
            .HasColumnName("Slug")
            .HasMaxLength(63)
            .IsRequired();

        builder.HasIndex(s => s.Slug).IsUnique();

        builder.HasMany(s => s.Tags)
            .WithOne(t => t.Service)
            .HasForeignKey(t => t.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Dependencies)
            .WithOne(d => d.Service)
            .HasForeignKey(d => d.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(s => s.DomainEvents);
    }
}
