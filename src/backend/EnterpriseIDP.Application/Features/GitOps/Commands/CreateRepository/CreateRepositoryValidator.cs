using FluentValidation;

namespace EnterpriseIDP.Application.Features.GitOps.Commands.CreateRepository;

public class CreateRepositoryValidator : AbstractValidator<CreateRepositoryCommand>
{
    public CreateRepositoryValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100)
            .Matches(@"^[a-zA-Z0-9\-_\.]+$").WithMessage("Invalid repository name.");
        RuleFor(x => x.ServiceId).NotEmpty();
        RuleFor(x => x.TeamId).NotEmpty();
    }
}
