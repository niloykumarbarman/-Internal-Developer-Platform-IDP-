using EnterpriseIDP.Domain.Enums;
using MediatR;

namespace EnterpriseIDP.Application.Features.Kubernetes.Commands.CreateNamespace;

public record CreateNamespaceCommand(
    string Name,
    Guid TeamId,
    EnvironmentType Environment,
    string CpuLimit = "2",
    string MemoryLimit = "4Gi"
) : IRequest<CreateNamespaceResult>;

public record CreateNamespaceResult(
    Guid Id, string Name, Guid TeamId,
    EnvironmentType Environment, DateTime CreatedAt
);
