using ErrorOr;

namespace EnterpriseIDP.Domain.ValueObjects;

public sealed class Email
{
    public string Value { get; }

    private Email(string value) => Value = value;

    public static ErrorOr<Email> Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Error.Validation("Email.Empty", "Email cannot be empty.");

        if (!email.Contains('@') || !email.Contains('.'))
            return Error.Validation("Email.Invalid", "Email format is invalid.");

        if (email.Length > 256)
            return Error.Validation("Email.TooLong", "Email must be 256 characters or less.");

        return new Email(email.ToLowerInvariant().Trim());
    }

    public override string ToString() => Value;
    public override bool Equals(object? obj) => obj is Email e && Value == e.Value;
    public override int GetHashCode() => Value.GetHashCode();
}
