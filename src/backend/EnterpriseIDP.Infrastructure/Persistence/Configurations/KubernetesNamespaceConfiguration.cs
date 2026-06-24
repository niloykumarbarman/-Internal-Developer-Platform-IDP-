using EnterpriseIDP.Domain.Entities.Kubernetes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseIDP.Infrastructure.Persistence.Configurations;

public class KubernetesNamespaceConfiguration : IEntityTypeConfiguration<KubernetesNamespace>
{
    public void Configure(EntityTypeBuilder<KubernetesNamespace> builder)
    {
        builder.ToTable("KubernetesNamespaces");
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Name).IsRequired().HasMaxLength(100);
        builder.Property(n => n.Environment).HasConversion<string>().HasMaxLength(50);
        builder.HasIndex(n => n.Name).IsUnique();

        builder.HasMany(n => n.Deployments)
            .WithOne(d => d.Namespace)
            .HasForeignKey(d => d.NamespaceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(n => n.DomainEvents);
    }
}
