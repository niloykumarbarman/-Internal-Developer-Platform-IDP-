using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using EnterpriseIDP.Application.Features.Auth.Commands.CreateTeam;
using EnterpriseIDP.Application.Features.Auth.Commands.RegisterUser;
using EnterpriseIDP.Application.Features.Catalog.Commands.RegisterService;
using EnterpriseIDP.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace EnterpriseIDP.Tests.Integration;

public class CatalogControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public CatalogControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<(HttpClient Client, Guid TeamId)> GetAuthenticatedClientWithTeamAsync()
    {
        var email = $"catalog_{Guid.NewGuid():N}@example.com";
        var registerCommand = new RegisterUserCommand("Test", "User", email, "StrongP@ss1", "PlatformEngineer");
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerCommand);
        var registerResult = await registerResponse.Content.ReadFromJsonAsync<RegisterUserResult>();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", registerResult!.Token);

        var teamCommand = new CreateTeamCommand($"Team-{Guid.NewGuid():N}".Substring(0, 12), "Integration test team", registerResult.Id);
        var teamResponse = await _client.PostAsJsonAsync("/api/auth/teams", teamCommand);
        teamResponse.StatusCode.Should().Be(HttpStatusCode.OK, "team creation must succeed for catalog tests to proceed");
        var teamResult = await teamResponse.Content.ReadFromJsonAsync<CreateTeamResult>();

        return (_client, teamResult!.Id);
    }

    private static RegisterServiceCommand ValidCommand(Guid teamId, string? name = null) => new(
        Name: name ?? $"service-{Guid.NewGuid():N}".Substring(0, 16),
        Description: "Integration test service",
        Type: ServiceType.WebApi,
        Owner: "Platform Team",
        TeamId: teamId,
        RepositoryUrl: "https://github.com/org/test-service",
        DocumentationUrl: null,
        Tags: new List<string> { "test", "integration" }
    );

    [Fact]
    public async Task RegisterService_WithoutAuth_ReturnsUnauthorized()
    {
        using var anonymousClient = _factory.CreateClient();

        var response = await anonymousClient.PostAsJsonAsync("/api/services", ValidCommand(Guid.NewGuid()));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RegisterService_WithValidData_ReturnsOkWithService()
    {
        var (client, teamId) = await GetAuthenticatedClientWithTeamAsync();
        var command = ValidCommand(teamId);

        var response = await client.PostAsJsonAsync("/api/services", command);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<RegisterServiceResult>(JsonOptions);
        result.Should().NotBeNull();
        result!.Name.Should().Be(command.Name);
        result.Type.Should().Be(ServiceType.WebApi);
        result.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task RegisterService_WithNonExistentTeam_ReturnsNotFound()
    {
        var (client, _) = await GetAuthenticatedClientWithTeamAsync();
        var command = ValidCommand(Guid.NewGuid());

        var response = await client.PostAsJsonAsync("/api/services", command);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RegisterService_WithDuplicateName_ReturnsConflict()
    {
        var (client, teamId) = await GetAuthenticatedClientWithTeamAsync();
        var command = ValidCommand(teamId);
        await client.PostAsJsonAsync("/api/services", command);

        var response = await client.PostAsJsonAsync("/api/services", command);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetServiceById_AfterRegistering_ReturnsSameService()
    {
        var (client, teamId) = await GetAuthenticatedClientWithTeamAsync();
        var command = ValidCommand(teamId);
        var registerResponse = await client.PostAsJsonAsync("/api/services", command);
        var registered = await registerResponse.Content.ReadFromJsonAsync<RegisterServiceResult>(JsonOptions);

        var response = await client.GetAsync($"/api/services/{registered!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetServices_ReturnsOkWithList()
    {
        var (client, teamId) = await GetAuthenticatedClientWithTeamAsync();
        await client.PostAsJsonAsync("/api/services", ValidCommand(teamId));

        var response = await client.GetAsync("/api/services");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
