using EnterpriseIDP.Domain.Common;

namespace EnterpriseIDP.Domain.Events.Catalog;

public sealed class ServiceRegisteredEvent : DomainEvent
{
    public Guid ServiceId { get; }
    public string ServiceName { get; }
    public string Slug { get; }
    public Guid OwnerTeamId { get; }
    public override string EventType => "service.registered";

    public ServiceRegisteredEvent(Guid serviceId, string serviceName, string slug, Guid ownerTeamId)
    {
        ServiceId = serviceId;
        ServiceName = serviceName;
        Slug = slug;
        OwnerTeamId = ownerTeamId;
    }
}
