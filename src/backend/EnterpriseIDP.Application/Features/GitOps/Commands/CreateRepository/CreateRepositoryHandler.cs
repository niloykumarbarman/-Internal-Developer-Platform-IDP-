using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace EnterpriseIDP.Application.Features.GitOps.Commands.CreateRepository;

public class CreateRepositoryHandler : IRequestHandler<CreateRepositoryCommand, CreateRepositoryResult>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CreateRepositoryHandler> _logger;

    public CreateRepositoryHandler(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<CreateRepositoryHandler> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<CreateRepositoryResult> Handle(
        CreateRepositoryCommand request,
        CancellationToken cancellationToken)
    {
        var githubToken = _configuration["GitHub:Token"];
        var githubOrg = _configuration["GitHub:Organization"];

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", githubToken);
        client.DefaultRequestHeaders.Add("User-Agent", "EnterpriseIDP");
        client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");

        var payload = new
        {
            name = request.RepositoryName,
            description = request.Description,
            @private = request.IsPrivate,
            auto_init = true,
            gitignore_template = "VisualStudio"
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var url = string.IsNullOrEmpty(githubOrg)
            ? "https://api.github.com/user/repos"
            : $"https://api.github.com/orgs/{githubOrg}/repos";

        var response = await client.PostAsync(url, content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("GitHub repo creation failed: {Error}", error);
            return new CreateRepositoryResult(false, "", "", "");
        }

        var result = await response.Content.ReadFromJsonAsync<JsonElement>(
            cancellationToken: cancellationToken);

        return new CreateRepositoryResult(
            Success: true,
            RepositoryUrl: result.GetProperty("html_url").GetString() ?? "",
            CloneUrl: result.GetProperty("clone_url").GetString() ?? "",
            DefaultBranch: result.GetProperty("default_branch").GetString() ?? "main"
        );
    }
}
