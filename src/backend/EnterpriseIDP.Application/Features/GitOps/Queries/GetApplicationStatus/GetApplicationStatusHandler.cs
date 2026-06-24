using MediatR;
using Microsoft.Extensions.Logging;

namespace EnterpriseIDP.Application.Features.GitOps.Queries.GetApplicationStatus;

public class GetApplicationStatusHandler 
    : IRequestHandler<GetApplicationStatusQuery, ApplicationStatusDto>
{
    private readonly ILogger<GetApplicationStatusHandler> _logger;

    public GetApplicationStatusHandler(ILogger<GetApplicationStatusHandler> logger)
    {
        _logger = logger;
    }

    public async Task<ApplicationStatusDto> Handle(
        GetApplicationStatusQuery request,
        CancellationToken cancellationToken)
    {
        // In production: call ArgoCD REST API GET /api/v1/applications/{name}
        await Task.Delay(50, cancellationToken);

        return new ApplicationStatusDto(
            Name: request.ApplicationName,
            SyncStatus: "Synced",
            HealthStatus: "Healthy",
            Namespace: "enterprise-idp",
            RepoUrl: "https://github.com/niloykumarbarman/-Internal-Developer-Platform-IDP-",
            TargetRevision: "main",
            LastSyncedAt: DateTime.UtcNow
        );
    }
}
