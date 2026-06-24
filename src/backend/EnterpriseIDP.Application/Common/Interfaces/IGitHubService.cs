namespace EnterpriseIDP.Application.Common.Interfaces;

public interface IGitHubService
{
    Task<(long RepoId, string CloneUrl, string HtmlUrl)> CreateRepositoryAsync(string repoName, string description, bool isPrivate, CancellationToken ct = default);
    Task<bool> RepositoryExistsAsync(string repoName, CancellationToken ct = default);
    Task DeleteRepositoryAsync(string repoName, CancellationToken ct = default);
    string OrgName { get; }
}
