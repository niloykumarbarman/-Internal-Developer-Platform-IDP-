using EnterpriseIDP.Domain.Common;

namespace EnterpriseIDP.Domain.Entities.Catalog;

public sealed class ServiceTag : BaseEntity
{
    public Guid ServiceId { get; private set; }
    public string Key { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;

    public Service Service { get; private set; } = null!;

    private ServiceTag() { }

    public static ServiceTag Create(Guid serviceId, string key, string value, string createdBy)
    {
        return new ServiceTag
        {
            ServiceId = serviceId,
            Key = key.ToLowerInvariant().Trim(),
            Value = value.Trim(),
            CreatedBy = createdBy
        };
    }
}
