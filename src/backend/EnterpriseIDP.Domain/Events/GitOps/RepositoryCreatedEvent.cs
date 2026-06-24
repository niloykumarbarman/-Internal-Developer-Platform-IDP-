using EnterpriseIDP.Domain.Common;

namespace EnterpriseIDP.Domain.Events.GitOps;

public sealed class RepositoryCreatedEvent : DomainEvent
{
    public Guid RepositoryId { get; }
    public string FullName { get; }
    public Guid ServiceId { get; }
    public Guid TeamId { get; }
    public override string EventType => "repository.created";

    public RepositoryCreatedEvent(Guid repositoryId, string fullName, Guid serviceId, Guid teamId)
    {
        RepositoryId = repositoryId;
        FullName = fullName;
        ServiceId = serviceId;
        TeamId = teamId;
    }
}
