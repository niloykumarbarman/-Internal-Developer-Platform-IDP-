using MediatR;

namespace EnterpriseIDP.Application.Features.GitOps.Commands.CreateRepository;

public record CreateRepositoryCommand(
    string RepositoryName,
    string Description,
    string TeamId,
    bool IsPrivate = true,
    string Template = "microservice-dotnet"
) : IRequest<CreateRepositoryResult>;

public record CreateRepositoryResult(
    bool Success,
    string RepositoryUrl,
    string CloneUrl,
    string DefaultBranch
);
