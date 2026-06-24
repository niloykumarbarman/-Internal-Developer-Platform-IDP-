using EnterpriseIDP.Domain.Common;
using EnterpriseIDP.Domain.Events.GitOps;
using ErrorOr;

namespace EnterpriseIDP.Domain.Entities.GitOps;

public sealed class Repository : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsPrivate { get; private set; }
    public string DefaultBranch { get; private set; } = "main";
    public string CloneUrl { get; private set; } = string.Empty;
    public string HtmlUrl { get; private set; } = string.Empty;
    public Guid ServiceId { get; private set; }
    public Guid TeamId { get; private set; }
    public long GitHubRepoId { get; private set; }
    public string? Language { get; private set; }
    public bool ArgocdSyncEnabled { get; private set; }
    public string? ArgocdAppName { get; private set; }

    private Repository() { }

    public static ErrorOr<Repository> Create(
        string name,
        string fullName,
        string cloneUrl,
        string htmlUrl,
        long gitHubRepoId,
        Guid serviceId,
        Guid teamId,
        bool isPrivate,
        string createdBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Error.Validation("Repository.Name", "Repository name cannot be empty.");

        var repo = new Repository
        {
            Name = name.Trim(),
            FullName = fullName.Trim(),
            CloneUrl = cloneUrl,
            HtmlUrl = htmlUrl,
            GitHubRepoId = gitHubRepoId,
            ServiceId = serviceId,
            TeamId = teamId,
            IsPrivate = isPrivate,
            CreatedBy = createdBy
        };

        repo.AddDomainEvent(new RepositoryCreatedEvent(repo.Id, repo.FullName, serviceId, teamId));
        return repo;
    }

    public void EnableArgocd(string appName, string updatedBy)
    {
        ArgocdSyncEnabled = true;
        ArgocdAppName = appName;
        SetUpdated(updatedBy);
    }
}
