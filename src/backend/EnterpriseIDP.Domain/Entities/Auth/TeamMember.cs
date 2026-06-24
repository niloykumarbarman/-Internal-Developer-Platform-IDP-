using EnterpriseIDP.Domain.Common;

namespace EnterpriseIDP.Domain.Entities.Auth;

public sealed class TeamMember : BaseEntity
{
    public Guid TeamId { get; private set; }
    public Guid UserId { get; private set; }
    public string Role { get; private set; } = string.Empty;
    public DateTime JoinedAt { get; private set; }

    public Team Team { get; private set; } = null!;
    public User User { get; private set; } = null!;

    private TeamMember() { }

    public static TeamMember Create(Guid teamId, Guid userId, string role, string createdBy)
    {
        return new TeamMember
        {
            TeamId = teamId,
            UserId = userId,
            Role = role,
            JoinedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }
}
