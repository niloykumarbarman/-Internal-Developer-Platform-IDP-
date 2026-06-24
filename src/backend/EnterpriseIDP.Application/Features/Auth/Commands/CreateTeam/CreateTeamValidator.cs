using FluentValidation;

namespace EnterpriseIDP.Application.Features.Auth.Commands.CreateTeam;

public class CreateTeamValidator : AbstractValidator<CreateTeamCommand>
{
    public CreateTeamValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.OwnerId).NotEmpty();
    }
}
