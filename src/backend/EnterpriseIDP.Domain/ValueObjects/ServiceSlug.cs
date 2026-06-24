using ErrorOr;
using System.Text.RegularExpressions;

namespace EnterpriseIDP.Domain.ValueObjects;

public sealed class ServiceSlug
{
    public string Value { get; }

    private static readonly Regex SlugRegex = new(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", RegexOptions.Compiled);

    private ServiceSlug(string value) => Value = value;

    public static ErrorOr<ServiceSlug> Create(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return Error.Validation("Slug.Empty", "Slug cannot be empty.");

        if (slug.Length < 3 || slug.Length > 63)
            return Error.Validation("Slug.Length", "Slug must be between 3 and 63 characters.");

        if (!SlugRegex.IsMatch(slug))
            return Error.Validation("Slug.Invalid", "Slug must be lowercase alphanumeric with hyphens only.");

        return new ServiceSlug(slug);
    }

    public override string ToString() => Value;
    public override bool Equals(object? obj) => obj is ServiceSlug s && Value == s.Value;
    public override int GetHashCode() => Value.GetHashCode();
}
