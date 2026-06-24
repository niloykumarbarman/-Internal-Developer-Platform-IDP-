using FluentValidation;

namespace EnterpriseIDP.Application.Features.Catalog.Commands.RegisterService;

public class RegisterServiceValidator : AbstractValidator<RegisterServiceCommand>
{
    public RegisterServiceValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100)
            .Matches(@"^[a-zA-Z0-9\-_]+$").WithMessage("Name can only contain letters, numbers, hyphens, and underscores.");
        RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Owner).NotEmpty().MaximumLength(100);
        RuleFor(x => x.TeamId).NotEmpty();
        RuleFor(x => x.RepositoryUrl).Must(u => u == null || Uri.IsWellFormedUriString(u, UriKind.Absolute))
            .WithMessage("Invalid repository URL.");
    }
}
