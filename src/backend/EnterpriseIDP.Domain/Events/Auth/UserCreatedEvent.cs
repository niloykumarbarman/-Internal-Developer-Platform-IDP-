using EnterpriseIDP.Domain.Common;
using EnterpriseIDP.Domain.Enums;

namespace EnterpriseIDP.Domain.Events.Auth;

public sealed class UserCreatedEvent : DomainEvent
{
    public Guid UserId { get; }
    public string Email { get; }
    public UserRole Role { get; }
    public override string EventType => "user.created";

    public UserCreatedEvent(Guid userId, string email, UserRole role)
    {
        UserId = userId;
        Email = email;
        Role = role;
    }
}
