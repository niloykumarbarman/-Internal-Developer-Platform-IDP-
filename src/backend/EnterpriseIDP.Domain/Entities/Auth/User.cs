using EnterpriseIDP.Domain.Common;
using EnterpriseIDP.Domain.Enums;
using EnterpriseIDP.Domain.ValueObjects;
using EnterpriseIDP.Domain.Events.Auth;
using ErrorOr;

namespace EnterpriseIDP.Domain.Entities.Auth;

public sealed class User : BaseEntity
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool IsEmailVerified { get; private set; } = false;
    public string? GitHubUsername { get; private set; }
    public string? GitHubAccessToken { get; private set; }
    public string? AvatarUrl { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiry { get; private set; }

    private readonly List<TeamMember> _teamMemberships = [];
    public IReadOnlyList<TeamMember> TeamMemberships => _teamMemberships.AsReadOnly();

    private User() { }

    public static ErrorOr<User> Create(
        string firstName,
        string lastName,
        string email,
        string passwordHash,
        UserRole role = UserRole.Developer,
        string createdBy = "system")
    {
        var emailResult = Email.Create(email);
        if (emailResult.IsError) return emailResult.Errors;

        var user = new User
        {
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Email = emailResult.Value,
            PasswordHash = passwordHash,
            Role = role,
            CreatedBy = createdBy
        };

        user.AddDomainEvent(new UserCreatedEvent(user.Id, user.Email.Value, user.Role));
        return user;
    }

    public ErrorOr<Success> UpdateProfile(string firstName, string lastName, string? avatarUrl, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Error.Validation("User.FirstName", "First name cannot be empty.");

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        AvatarUrl = avatarUrl;
        SetUpdated(updatedBy);
        return Result.Success;
    }

    public void SetGitHubInfo(string username, string accessToken, string? avatarUrl)
    {
        GitHubUsername = username;
        GitHubAccessToken = accessToken;
        AvatarUrl ??= avatarUrl;
    }

    public void SetRefreshToken(string token, DateTime expiry)
    {
        RefreshToken = token;
        RefreshTokenExpiry = expiry;
    }

    public void RecordLogin() => LastLoginAt = DateTime.UtcNow;

    public void ChangeRole(UserRole newRole, string updatedBy)
    {
        Role = newRole;
        SetUpdated(updatedBy);
    }

    public void Deactivate(string updatedBy)
    {
        IsActive = false;
        SetUpdated(updatedBy);
    }

    public void VerifyEmail() => IsEmailVerified = true;

    public string FullName => $"{FirstName} {LastName}";
}
