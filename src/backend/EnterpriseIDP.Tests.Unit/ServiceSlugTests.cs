using EnterpriseIDP.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace EnterpriseIDP.Tests.Unit;

public class ServiceSlugTests
{
    [Theory]
    [InlineData("api-gateway")]
    [InlineData("auth-service")]
    [InlineData("svc1")]
    public void Create_WithValidSlug_ReturnsSuccess(string slug)
    {
        var result = ServiceSlug.Create(slug);

        result.IsError.Should().BeFalse();
        result.Value.Value.Should().Be(slug);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptySlug_ReturnsValidationError(string slug)
    {
        var result = ServiceSlug.Create(slug);

        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Slug.Empty");
    }

    [Theory]
    [InlineData("ab")]
    public void Create_TooShort_ReturnsValidationError(string slug)
    {
        var result = ServiceSlug.Create(slug);

        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Slug.Length");
    }

    [Theory]
    [InlineData("Api-Gateway")]
    [InlineData("api_gateway")]
    [InlineData("api gateway")]
    [InlineData("-api-gateway")]
    [InlineData("api-gateway-")]
    public void Create_WithInvalidFormat_ReturnsValidationError(string slug)
    {
        var result = ServiceSlug.Create(slug);

        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Slug.Invalid");
    }
}
