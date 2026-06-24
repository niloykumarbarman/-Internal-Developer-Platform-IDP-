using EnterpriseIDP.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Octokit;

namespace EnterpriseIDP.Infrastructure.Services;

public class GitHubService : IGitHubService
{
    private readonly GitHubClient _client;
    public string OrgName { get; }

    public GitHubService(IConfiguration configuration)
    {
        OrgName = configuration["GitHub:OrgName"] ?? string.Empty;
        var token = configuration["GitHub:AccessToken"] ?? string.Empty;

        _client = new GitHubClient(new ProductHeaderValue("EnterpriseIDP"));
        if (!string.IsNullOrWhiteSpace(token))
        {
            _client.Credentials = new Credentials(token);
        }
    }

    public async Task<(long RepoId, string CloneUrl, string HtmlUrl)> CreateRepositoryAsync(
        string repoName, string description, bool isPrivate, CancellationToken ct = default)
    {
        var newRepo = new NewRepository(repoName)
        {
            Description = description,
            Private = isPrivate,
            AutoInit = true
        };

        Repository repo = string.IsNullOrWhiteSpace(OrgName)
            ? await _client.Repository.Create(newRepo)
            : await _client.Repository.Create(OrgName, newRepo);

        return (repo.Id, repo.CloneUrl, repo.HtmlUrl);
    }

    public async Task<bool> RepositoryExistsAsync(string repoName, CancellationToken ct = default)
    {
        try
        {
            var owner = string.IsNullOrWhiteSpace(OrgName) ? (await _client.User.Current()).Login : OrgName;
            await _client.Repository.Get(owner, repoName);
            return true;
        }
        catch (NotFoundException)
        {
            return false;
        }
    }

    public async Task DeleteRepositoryAsync(string repoName, CancellationToken ct = default)
    {
        var owner = string.IsNullOrWhiteSpace(OrgName) ? (await _client.User.Current()).Login : OrgName;
        await _client.Repository.Delete(owner, repoName);
    }
}
