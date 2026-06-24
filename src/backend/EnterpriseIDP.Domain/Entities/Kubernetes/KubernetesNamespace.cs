using EnterpriseIDP.Domain.Common;
using EnterpriseIDP.Domain.Enums;
using ErrorOr;

namespace EnterpriseIDP.Domain.Entities.Kubernetes;

public sealed class KubernetesNamespace : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public EnvironmentType Environment { get; private set; }
    public Guid TeamId { get; private set; }
    public string? CpuLimit { get; private set; }
    public string? MemoryLimit { get; private set; }
    public string? CpuRequest { get; private set; }
    public string? MemoryRequest { get; private set; }
    public bool IsProvisioned { get; private set; }
    public string? ClusterName { get; private set; }
    public int MaxPods { get; private set; } = 10;

    private readonly List<KubernetesDeployment> _deployments = [];
    public IReadOnlyList<KubernetesDeployment> Deployments => _deployments.AsReadOnly();

    private KubernetesNamespace() { }

    public static ErrorOr<KubernetesNamespace> Create(
        string name,
        EnvironmentType environment,
        Guid teamId,
        string? clusterName,
        string createdBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Error.Validation("Namespace.Name", "Namespace name cannot be empty.");

        return new KubernetesNamespace
        {
            Name = name.ToLowerInvariant().Trim(),
            Environment = environment,
            TeamId = teamId,
            ClusterName = clusterName,
            CreatedBy = createdBy
        };
    }

    public void SetResourceQuota(string cpuLimit, string memoryLimit, string cpuRequest, string memoryRequest, int maxPods, string updatedBy)
    {
        CpuLimit = cpuLimit;
        MemoryLimit = memoryLimit;
        CpuRequest = cpuRequest;
        MemoryRequest = memoryRequest;
        MaxPods = maxPods;
        SetUpdated(updatedBy);
    }

    public void MarkProvisioned(string updatedBy)
    {
        IsProvisioned = true;
        SetUpdated(updatedBy);
    }
}
