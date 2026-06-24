using MediatR;

namespace EnterpriseIDP.Application.Features.GitOps.Commands.SyncApplication;

public record SyncApplicationCommand(
    string ApplicationName,
    string Namespace,
    bool DryRun = false
) : IRequest<SyncApplicationResult>;

public record SyncApplicationResult(
    bool Success,
    string Message,
    string SyncStatus,
    DateTime SyncedAt
);
