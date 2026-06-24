using EnterpriseIDP.Domain.Common;

namespace EnterpriseIDP.Domain.Entities.Catalog;

public sealed class ServiceDependency : BaseEntity
{
    public Guid ServiceId { get; private set; }
    public Guid DependsOnServiceId { get; private set; }

    public Service Service { get; private set; } = null!;
    public Service DependsOnService { get; private set; } = null!;

    private ServiceDependency() { }

    public static ServiceDependency Create(Guid serviceId, Guid dependsOnServiceId, string createdBy)
    {
        return new ServiceDependency
        {
            ServiceId = serviceId,
            DependsOnServiceId = dependsOnServiceId,
            CreatedBy = createdBy
        };
    }
}
