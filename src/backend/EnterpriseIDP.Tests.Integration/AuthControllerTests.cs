using System.Net;
using System.Net.Http.Json;
using EnterpriseIDP.Application.Features.Auth.Commands.LoginUser;
using EnterpriseIDP.Application.Features.Auth.Commands.RegisterUser;
using FluentAssertions;
using Xunit;

namespace EnterpriseIDP.Tests.Integration;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsOk()
    {
        var command = new RegisterUserCommand(
            FirstName: "Test",
            LastName: "User",
            Email: $"test_{Guid.NewGuid():N}@example.com",
            Password: "StrongP@ss1",
            Role: "Developer"
        );

        var response = await _client.PostAsJsonAsync("/api/auth/register", command);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<RegisterUserResult>();
        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrEmpty();
        result.Role.Should().Be("Developer");
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ReturnsConflict()
    {
        var email = $"dup_{Guid.NewGuid():N}@example.com";
        var command = new RegisterUserCommand("Test", "User", email, "StrongP@ss1", "Developer");

        await _client.PostAsJsonAsync("/api/auth/register", command);
        var response = await _client.PostAsJsonAsync("/api/auth/register", command);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.Conflict, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkWithToken()
    {
        var email = $"login_{Guid.NewGuid():N}@example.com";
        var password = "StrongP@ss1";
        var registerCommand = new RegisterUserCommand("Test", "User", email, password, "Developer");
        await _client.PostAsJsonAsync("/api/auth/register", registerCommand);

        var loginCommand = new LoginUserCommand(email, password);
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginCommand);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<LoginUserResult>();
        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrEmpty();
        result.Email.Should().Be(email);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
    {
        var email = $"badpw_{Guid.NewGuid():N}@example.com";
        var registerCommand = new RegisterUserCommand("Test", "User", email, "StrongP@ss1", "Developer");
        await _client.PostAsJsonAsync("/api/auth/register", registerCommand);

        var loginCommand = new LoginUserCommand(email, "WrongPassword999");
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginCommand);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithNonExistentUser_ReturnsUnauthorized()
    {
        var loginCommand = new LoginUserCommand($"nouser_{Guid.NewGuid():N}@example.com", "SomePassword1");
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginCommand);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
