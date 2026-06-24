using EnterpriseIDP.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseIDP.Infrastructure.Persistence.Configurations;

public class ServiceDependencyConfiguration : IEntityTypeConfiguration<ServiceDependency>
{
    public void Configure(EntityTypeBuilder<ServiceDependency> builder)
    {
        builder.ToTable("ServiceDependencies");
        builder.HasKey(d => d.Id);

        builder.HasOne(d => d.DependsOnService)
            .WithMany()
            .HasForeignKey(d => d.DependsOnServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(d => d.DomainEvents);
    }
}
