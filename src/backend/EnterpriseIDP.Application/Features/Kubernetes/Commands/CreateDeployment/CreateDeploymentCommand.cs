using EnterpriseIDP.Domain.Enums;
using MediatR;

namespace EnterpriseIDP.Application.Features.Kubernetes.Commands.CreateDeployment;

public record CreateDeploymentCommand(
    string Name,
    Guid ServiceId,
    Guid NamespaceId,
    string ImageName,
    string ImageTag,
    EnvironmentType Environment,
    int Replicas = 1
) : IRequest<CreateDeploymentResult>;

public record CreateDeploymentResult(
    Guid Id,
    string Name,
    Guid ServiceId,
    Guid NamespaceId,
    string ImageName,
    string ImageTag,
    int Replicas,
    DeploymentStatus Status,
    EnvironmentType Environment,
    DateTime CreatedAt
);
