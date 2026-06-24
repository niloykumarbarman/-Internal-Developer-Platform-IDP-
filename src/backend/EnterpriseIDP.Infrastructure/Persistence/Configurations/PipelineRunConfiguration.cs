using EnterpriseIDP.Domain.Entities.CICD;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseIDP.Infrastructure.Persistence.Configurations;

public class PipelineRunConfiguration : IEntityTypeConfiguration<PipelineRun>
{
    public void Configure(EntityTypeBuilder<PipelineRun> builder)
    {
        builder.ToTable("PipelineRuns");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Status).HasConversion<string>().HasMaxLength(50);
        builder.Ignore(r => r.DomainEvents);
    }
}
