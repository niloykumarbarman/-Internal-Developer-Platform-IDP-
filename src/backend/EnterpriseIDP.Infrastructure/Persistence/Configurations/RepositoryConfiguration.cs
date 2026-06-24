using EnterpriseIDP.Domain.Entities.GitOps;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseIDP.Infrastructure.Persistence.Configurations;

public class RepositoryConfiguration : IEntityTypeConfiguration<Repository>
{
    public void Configure(EntityTypeBuilder<Repository> builder)
    {
        builder.ToTable("Repositories");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name).IsRequired().HasMaxLength(200);
        builder.Property(r => r.FullName).IsRequired().HasMaxLength(300);
        builder.Property(r => r.CloneUrl).IsRequired();
        builder.Property(r => r.HtmlUrl).IsRequired();

        builder.HasIndex(r => r.GitHubRepoId).IsUnique();

        builder.Ignore(r => r.DomainEvents);
    }
}
