using FluentValidation;

namespace EnterpriseIDP.Application.Features.Auth.Commands.RegisterUser;

public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8)
            .Matches(@"[A-Z]").WithMessage("Password must contain uppercase.")
            .Matches(@"[0-9]").WithMessage("Password must contain a digit.");
        RuleFor(x => x.Role).Must(r => new[] { "Admin", "PlatformEngineer", "Developer" }.Contains(r))
            .WithMessage("Invalid role.");
    }
}
