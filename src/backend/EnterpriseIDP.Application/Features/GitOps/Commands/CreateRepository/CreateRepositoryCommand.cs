using MediatR;

namespace EnterpriseIDP.Application.Features.GitOps.Commands.CreateRepository;

public record CreateRepositoryCommand(
    string Name,
    string Description,
    Guid ServiceId,
    Guid TeamId,
    bool IsPrivate = true,
    string DefaultBranch = "main"
) : IRequest<CreateRepositoryResult>;

public record CreateRepositoryResult(
    Guid Id, string Name, string FullName,
    string CloneUrl, string DefaultBranch, DateTime CreatedAt
);
