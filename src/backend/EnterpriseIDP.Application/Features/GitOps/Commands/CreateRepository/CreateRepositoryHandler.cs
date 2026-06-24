using EnterpriseIDP.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EnterpriseIDP.Application.Features.GitOps.Commands.CreateRepository;

public class CreateRepositoryHandler
    : IRequestHandler<CreateRepositoryCommand, CreateRepositoryResult>
{
    private readonly IGitHubService _gitHubService;
    private readonly ILogger<CreateRepositoryHandler> _logger;

    public CreateRepositoryHandler(
        IGitHubService gitHubService,
        ILogger<CreateRepositoryHandler> logger)
    {
        _gitHubService = gitHubService;
        _logger = logger;
    }

    public async Task<CreateRepositoryResult> Handle(
        CreateRepositoryCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating GitHub repository {Name} for team {TeamId}",
            request.RepositoryName, request.TeamId);

        var (_, cloneUrl, htmlUrl) = await _gitHubService.CreateRepositoryAsync(
            request.RepositoryName,
            request.Description,
            request.IsPrivate,
            cancellationToken);

        return new CreateRepositoryResult(
            Success: true,
            RepositoryUrl: htmlUrl,
            CloneUrl: cloneUrl,
            DefaultBranch: "main"
        );
    }
}
