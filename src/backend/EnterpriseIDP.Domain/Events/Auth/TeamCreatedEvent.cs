using EnterpriseIDP.Domain.Common;

namespace EnterpriseIDP.Domain.Events.Auth;

public sealed class TeamCreatedEvent : DomainEvent
{
    public Guid TeamId { get; }
    public string TeamName { get; }
    public Guid OwnerId { get; }
    public override string EventType => "team.created";

    public TeamCreatedEvent(Guid teamId, string teamName, Guid ownerId)
    {
        TeamId = teamId;
        TeamName = teamName;
        OwnerId = ownerId;
    }
}
