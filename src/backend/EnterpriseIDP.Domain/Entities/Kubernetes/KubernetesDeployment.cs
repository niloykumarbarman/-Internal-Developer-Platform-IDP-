using EnterpriseIDP.Domain.Common;
using EnterpriseIDP.Domain.Enums;
using ErrorOr;

namespace EnterpriseIDP.Domain.Entities.Kubernetes;

public sealed class KubernetesDeployment : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public Guid ServiceId { get; private set; }
    public Guid NamespaceId { get; private set; }
    public string ImageName { get; private set; } = string.Empty;
    public string ImageTag { get; private set; } = string.Empty;
    public int Replicas { get; private set; }
    public DeploymentStatus Status { get; private set; }
    public EnvironmentType Environment { get; private set; }
    public string? HelmChart { get; private set; }
    public string? HelmValues { get; private set; }
    public DateTime? DeployedAt { get; private set; }
    public string? DeployedBy { get; private set; }
    public string? RollbackImageTag { get; private set; }

    public KubernetesNamespace Namespace { get; private set; } = null!;

    private KubernetesDeployment() { }

    public static ErrorOr<KubernetesDeployment> Create(
        string name,
        Guid serviceId,
        Guid namespaceId,
        string imageName,
        string imageTag,
        int replicas,
        EnvironmentType environment,
        string createdBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Error.Validation("Deployment.Name", "Deployment name cannot be empty.");

        if (replicas < 1 || replicas > 50)
            return Error.Validation("Deployment.Replicas", "Replicas must be between 1 and 50.");

        return new KubernetesDeployment
        {
            Name = name.Trim(),
            ServiceId = serviceId,
            NamespaceId = namespaceId,
            ImageName = imageName,
            ImageTag = imageTag,
            Replicas = replicas,
            Status = DeploymentStatus.Pending,
            Environment = environment,
            CreatedBy = createdBy
        };
    }

    public void MarkDeployed(string deployedBy)
    {
        RollbackImageTag = ImageTag;
        Status = DeploymentStatus.Running;
        DeployedAt = DateTime.UtcNow;
        DeployedBy = deployedBy;
    }

    public void UpdateImage(string newTag, string updatedBy)
    {
        RollbackImageTag = ImageTag;
        ImageTag = newTag;
        Status = DeploymentStatus.Pending;
        SetUpdated(updatedBy);
    }

    public void MarkFailed(string updatedBy)
    {
        Status = DeploymentStatus.Failed;
        SetUpdated(updatedBy);
    }

    public ErrorOr<Success> Rollback(string rolledBackBy)
    {
        if (RollbackImageTag is null)
            return Error.Validation("Deployment.NoRollback", "No previous version to rollback to.");

        ImageTag = RollbackImageTag;
        Status = DeploymentStatus.Pending;
        SetUpdated(rolledBackBy);
        return Result.Success;
    }

    public void ScaleReplicas(int replicas, string updatedBy)
    {
        Replicas = replicas;
        SetUpdated(updatedBy);
    }
}
