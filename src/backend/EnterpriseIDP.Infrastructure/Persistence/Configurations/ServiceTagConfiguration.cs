using EnterpriseIDP.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseIDP.Infrastructure.Persistence.Configurations;

public class ServiceTagConfiguration : IEntityTypeConfiguration<ServiceTag>
{
    public void Configure(EntityTypeBuilder<ServiceTag> builder)
    {
        builder.ToTable("ServiceTags");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Key).IsRequired().HasMaxLength(100);
        builder.Property(t => t.Value).IsRequired().HasMaxLength(200);
        builder.Ignore(t => t.DomainEvents);
    }
}
