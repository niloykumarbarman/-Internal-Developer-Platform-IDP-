using EnterpriseIDP.Domain.Enums;
using MediatR;

namespace EnterpriseIDP.Application.Features.Kubernetes.Queries.GetDeployments;

public record GetDeploymentsQuery(Guid? ServiceId = null, EnvironmentType? Environment = null) : IRequest<List<DeploymentDto>>;

public record DeploymentDto(
    Guid Id, string Name, string Namespace,
    string Image, string Tag, int Replicas,
    DeploymentStatus Status, EnvironmentType Environment,
    Guid ServiceId, DateTime CreatedAt
);
