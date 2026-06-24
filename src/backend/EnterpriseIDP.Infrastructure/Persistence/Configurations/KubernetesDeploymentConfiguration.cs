using EnterpriseIDP.Domain.Entities.Kubernetes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseIDP.Infrastructure.Persistence.Configurations;

public class KubernetesDeploymentConfiguration : IEntityTypeConfiguration<KubernetesDeployment>
{
    public void Configure(EntityTypeBuilder<KubernetesDeployment> builder)
    {
        builder.ToTable("KubernetesDeployments");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name).IsRequired().HasMaxLength(200);
        builder.Property(d => d.ImageName).IsRequired().HasMaxLength(300);
        builder.Property(d => d.ImageTag).IsRequired().HasMaxLength(100);
        builder.Property(d => d.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(d => d.Environment).HasConversion<string>().HasMaxLength(50);

        builder.Ignore(d => d.DomainEvents);
    }
}
