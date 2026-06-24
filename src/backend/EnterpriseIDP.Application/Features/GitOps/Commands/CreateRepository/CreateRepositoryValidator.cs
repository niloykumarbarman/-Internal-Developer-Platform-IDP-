using FluentValidation;

namespace EnterpriseIDP.Application.Features.GitOps.Commands.CreateRepository;

public class CreateRepositoryValidator : AbstractValidator<CreateRepositoryCommand>
{
    public CreateRepositoryValidator()
    {
        RuleFor(x => x.RepositoryName)
            .NotEmpty().WithMessage("Repository name is required")
            .MinimumLength(3).WithMessage("Minimum 3 characters")
            .MaximumLength(100).WithMessage("Maximum 100 characters")
            .Matches("^[a-zA-Z0-9-_]+$")
            .WithMessage("Only letters, numbers, hyphens, underscores allowed");

        RuleFor(x => x.TeamId)
            .NotEmpty().WithMessage("Team ID is required");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Maximum 500 characters");
    }
}
