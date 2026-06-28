using EnterpriseIDP.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace EnterpriseIDP.Tests.Unit;

public class EmailTests
{
    [Fact]
    public void Create_WithValidEmail_ReturnsSuccessAndNormalizesCase()
    {
        var result = Email.Create("  John.Doe@Example.com  ");

        result.IsError.Should().BeFalse();
        result.Value.Value.Should().Be("john.doe@example.com");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyEmail_ReturnsValidationError(string email)
    {
        var result = Email.Create(email);

        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Email.Empty");
    }

    [Theory]
    [InlineData("notanemail")]
    [InlineData("missing-at-sign.com")]
    [InlineData("missing-dot@com")]
    public void Create_WithInvalidFormat_ReturnsValidationError(string email)
    {
        var result = Email.Create(email);

        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Email.Invalid");
    }

    [Fact]
    public void Create_TooLong_ReturnsValidationError()
    {
        var longLocalPart = new string('a', 250);
        var email = $"{longLocalPart}@example.com";

        var result = Email.Create(email);

        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Email.TooLong");
    }
}
