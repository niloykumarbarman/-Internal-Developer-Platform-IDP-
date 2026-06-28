using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using EnterpriseIDP.Application.Features.Auth.Commands.CreateTeam;
using EnterpriseIDP.Application.Features.Auth.Commands.LoginUser;
using EnterpriseIDP.Application.Features.Auth.Commands.RegisterUser;
using EnterpriseIDP.Application.Features.Catalog.Commands.RegisterService;
using EnterpriseIDP.Application.Features.Catalog.Queries.GetServices;
using EnterpriseIDP.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace EnterpriseIDP.Tests.E2E;

public class UserJourneyTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly CustomWebApplicationFactory<Program> _factory;

    public UserJourneyTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task FullJourney_RegisterLoginCreateTeamRegisterService_Succeeds()
    {
        using var client = _factory.CreateClient();
        var email = $"e2e_{Guid.NewGuid():N}@example.com";
        const string password = "StrongP@ss1";

        // Step 1: Register a new user
        var registerCommand = new RegisterUserCommand("E2E", "Tester", email, password, "PlatformEngineer");
        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", registerCommand);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK, "registration must succeed to proceed with the journey");

        var registerResult = await registerResponse.Content.ReadFromJsonAsync<RegisterUserResult>(JsonOptions);
        registerResult.Should().NotBeNull();
        registerResult!.Email.Should().Be(email);

        // Step 2: Log in with the same credentials (separate from the registration token,
        // proves the login flow independently issues a valid token for this user)
        var loginCommand = new LoginUserCommand(email, password);
        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginCommand);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK, "login must succeed with the credentials just registered");

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginUserResult>(JsonOptions);
        loginResult.Should().NotBeNull();
        loginResult!.Token.Should().NotBeNullOrWhiteSpace();
        loginResult.Email.Should().Be(email);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.Token);

        // Step 3: Create a team using the login-issued token
        var teamName = $"E2E-Team-{Guid.NewGuid():N}".Substring(0, 16);
        var createTeamCommand = new CreateTeamCommand(teamName, "Team created during E2E journey test", registerResult.Id);
        var teamResponse = await client.PostAsJsonAsync("/api/auth/teams", createTeamCommand);
        teamResponse.StatusCode.Should().Be(HttpStatusCode.OK, "team creation must succeed for an authenticated user");

        var teamResult = await teamResponse.Content.ReadFromJsonAsync<CreateTeamResult>(JsonOptions);
        teamResult.Should().NotBeNull();
        teamResult!.Name.Should().Be(teamName);

        // Step 4: Register a service under the newly created team
        var serviceName = $"e2e-service-{Guid.NewGuid():N}".Substring(0, 20);
        var registerServiceCommand = new RegisterServiceCommand(
            Name: serviceName,
            Description: "Service registered during E2E journey test",
            Type: ServiceType.WebApi,
            Owner: "E2E Tester",
            TeamId: teamResult.Id,
            RepositoryUrl: "https://github.com/org/e2e-service",
            DocumentationUrl: null,
            Tags: new List<string> { "e2e" }
        );
        var registerServiceResponse = await client.PostAsJsonAsync("/api/services", registerServiceCommand);
        registerServiceResponse.StatusCode.Should().Be(HttpStatusCode.OK, "service registration must succeed for an authenticated team member");

        var registerServiceResult = await registerServiceResponse.Content.ReadFromJsonAsync<RegisterServiceResult>(JsonOptions);
        registerServiceResult.Should().NotBeNull();
        registerServiceResult!.Name.Should().Be(serviceName);

        // Step 5: Verify the service appears in the catalog listing, with the correct owner team name resolved
        var listResponse = await client.GetAsync("/api/services");
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK, "listing services must succeed for an authenticated user");

        var listResult = await listResponse.Content.ReadFromJsonAsync<GetServicesResult>(JsonOptions);
        listResult.Should().NotBeNull();
        var registered = listResult!.Items.SingleOrDefault(s => s.Id == registerServiceResult.Id);
        registered.Should().NotBeNull("the just-registered service must appear in the catalog listing");
        registered!.OwnerTeamName.Should().Be(teamName, "the catalog listing must resolve the real team name, not a placeholder or GUID");

        // Step 6: Verify the service can be fetched individually by ID
        var getByIdResponse = await client.GetAsync($"/api/services/{registerServiceResult.Id}");
        getByIdResponse.StatusCode.Should().Be(HttpStatusCode.OK, "fetching the registered service by ID must succeed");
    }

    [Fact]
    public async Task FullJourney_LoginWithWrongPassword_ReturnsUnauthorized()
    {
        using var client = _factory.CreateClient();
        var email = $"e2e_badpw_{Guid.NewGuid():N}@example.com";
        const string password = "StrongP@ss1";

        var registerCommand = new RegisterUserCommand("E2E", "BadPassword", email, password, "PlatformEngineer");
        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", registerCommand);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginCommand = new LoginUserCommand(email, "WrongPassword1!");
        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginCommand);

        loginResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized, "logging in with an incorrect password must be rejected");
    }

    [Fact]
    public async Task FullJourney_RegisterServiceWithoutTeam_ReturnsBadRequest()
    {
        using var client = _factory.CreateClient();
        var email = $"e2e_noteam_{Guid.NewGuid():N}@example.com";
        const string password = "StrongP@ss1";

        var registerCommand = new RegisterUserCommand("E2E", "NoTeam", email, password, "PlatformEngineer");
        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", registerCommand);
        var registerResult = await registerResponse.Content.ReadFromJsonAsync<RegisterUserResult>(JsonOptions);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", registerResult!.Token);

        // TeamId left as Guid.Empty — validator requires NotEmpty(), this must be rejected
        var command = new RegisterServiceCommand(
            Name: $"no-team-service-{Guid.NewGuid():N}".Substring(0, 18),
            Description: "Should fail validation",
            Type: ServiceType.WebApi,
            Owner: "Nobody",
            TeamId: Guid.Empty,
            RepositoryUrl: null,
            DocumentationUrl: null,
            Tags: new List<string>()
        );
        var response = await client.PostAsJsonAsync("/api/services", command);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest, "registering a service without a valid TeamId must fail validation");
    }
}
