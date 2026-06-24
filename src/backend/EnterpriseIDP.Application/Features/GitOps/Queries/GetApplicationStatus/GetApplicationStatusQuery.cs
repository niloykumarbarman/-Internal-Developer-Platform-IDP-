using MediatR;

namespace EnterpriseIDP.Application.Features.GitOps.Queries.GetApplicationStatus;

public record GetApplicationStatusQuery(string ApplicationName) 
    : IRequest<ApplicationStatusDto>;

public record ApplicationStatusDto(
    string Name,
    string SyncStatus,
    string HealthStatus,
    string Namespace,
    string RepoUrl,
    string TargetRevision,
    DateTime? LastSyncedAt
);
