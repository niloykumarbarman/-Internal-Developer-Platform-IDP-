using EnterpriseIDP.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseIDP.Infrastructure.Persistence.Configurations;

public class TeamMemberConfiguration : IEntityTypeConfiguration<TeamMember>
{
    public void Configure(EntityTypeBuilder<TeamMember> builder)
    {
        builder.ToTable("TeamMembers");
        builder.HasKey(tm => tm.Id);

        builder.Property(tm => tm.Role).IsRequired().HasMaxLength(50);

        builder.HasOne(tm => tm.User)
            .WithMany(u => u.TeamMemberships)
            .HasForeignKey(tm => tm.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(tm => new { tm.TeamId, tm.UserId }).IsUnique();

        builder.Ignore(tm => tm.DomainEvents);
    }
}
