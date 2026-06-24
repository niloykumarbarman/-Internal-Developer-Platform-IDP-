using EnterpriseIDP.Domain.Entities.CICD;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseIDP.Infrastructure.Persistence.Configurations;

public class PipelineConfiguration : IEntityTypeConfiguration<Pipeline>
{
    public void Configure(EntityTypeBuilder<Pipeline> builder)
    {
        builder.ToTable("Pipelines");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.RepositoryUrl).IsRequired();
        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(50);

        builder.HasMany(p => p.Runs)
            .WithOne(r => r.Pipeline)
            .HasForeignKey(r => r.PipelineId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(p => p.DomainEvents);
    }
}
