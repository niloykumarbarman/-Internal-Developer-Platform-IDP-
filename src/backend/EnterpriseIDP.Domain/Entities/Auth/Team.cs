using EnterpriseIDP.Domain.Common;
using EnterpriseIDP.Domain.Events.Auth;
using ErrorOr;

namespace EnterpriseIDP.Domain.Entities.Auth;

public sealed class Team : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? AvatarUrl { get; private set; }
    public Guid OwnerId { get; private set; }

    private readonly List<TeamMember> _members = [];
    public IReadOnlyList<TeamMember> Members => _members.AsReadOnly();

    private Team() { }

    public static ErrorOr<Team> Create(string name, string slug, Guid ownerId, string? description, string createdBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Error.Validation("Team.Name", "Team name cannot be empty.");

        if (string.IsNullOrWhiteSpace(slug))
            return Error.Validation("Team.Slug", "Team slug cannot be empty.");

        var team = new Team
        {
            Name = name.Trim(),
            Slug = slug.ToLowerInvariant().Trim(),
            Description = description,
            OwnerId = ownerId,
            CreatedBy = createdBy
        };

        team.AddDomainEvent(new TeamCreatedEvent(team.Id, team.Name, ownerId));
        return team;
    }

    public ErrorOr<Success> AddMember(Guid userId, string memberRole, string addedBy)
    {
        if (_members.Any(m => m.UserId == userId))
            return Error.Conflict("Team.MemberExists", "User is already a member of this team.");

        var member = TeamMember.Create(Id, userId, memberRole, addedBy);
        _members.Add(member);
        SetUpdated(addedBy);
        return Result.Success;
    }

    public ErrorOr<Success> RemoveMember(Guid userId, string removedBy)
    {
        var member = _members.FirstOrDefault(m => m.UserId == userId);
        if (member is null)
            return Error.NotFound("Team.MemberNotFound", "User is not a member of this team.");

        _members.Remove(member);
        SetUpdated(removedBy);
        return Result.Success;
    }

    public void Update(string name, string? description, string updatedBy)
    {
        Name = name.Trim();
        Description = description;
        SetUpdated(updatedBy);
    }
}
