using EnterpriseIDP.Application.Common.Exceptions;
using EnterpriseIDP.Application.Common.Extensions;
using EnterpriseIDP.Application.Common.Interfaces;
using EnterpriseIDP.Domain.Common;
using EnterpriseIDP.Domain.Entities.Catalog;
using EnterpriseIDP.Domain.Entities.GitOps;
using MediatR;

namespace EnterpriseIDP.Application.Features.GitOps.Commands.CreateRepository;

public class CreateRepositoryHandler : IRequestHandler<CreateRepositoryCommand, CreateRepositoryResult>
{
    private readonly IRepository<Repository> _repoRepo;
    private readonly IRepository<Service> _serviceRepo;
    private readonly IGitHubService _github;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public CreateRepositoryHandler(
        IRepository<Repository> repoRepo,
        IRepository<Service> serviceRepo,
        IGitHubService github,
        IUnitOfWork uow,
        ICurrentUserService currentUser)
    {
        _repoRepo = repoRepo;
        _serviceRepo = serviceRepo;
        _github = github;
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<CreateRepositoryResult> Handle(CreateRepositoryCommand cmd, CancellationToken ct)
    {
        var service = await _serviceRepo.GetByIdAsync(cmd.ServiceId, ct)
            ?? throw new NotFoundException(nameof(Service), cmd.ServiceId);

        if (await _github.RepositoryExistsAsync(cmd.Name, ct))
            throw new ConflictException($"Repository '{cmd.Name}' already exists.");

        var (repoId, cloneUrl, htmlUrl) = await _github.CreateRepositoryAsync(cmd.Name, cmd.Description, cmd.IsPrivate, ct);

        var createdBy = _currentUser.Email ?? "system";
        var repoResult = Repository.Create(
            name: cmd.Name,
            fullName: $"{_github.OrgName}/{cmd.Name}",
            cloneUrl: cloneUrl,
            htmlUrl: htmlUrl,
            gitHubRepoId: repoId,
            serviceId: cmd.ServiceId,
            teamId: cmd.TeamId,
            isPrivate: cmd.IsPrivate,
            createdBy: createdBy
        );

        var repo = repoResult.ThrowIfError();
        await _repoRepo.AddAsync(repo, ct);
        await _uow.SaveChangesAsync(ct);

        return new CreateRepositoryResult(repo.Id, repo.Name, repo.FullName, repo.CloneUrl, repo.DefaultBranch, repo.CreatedAt);
    }
}
