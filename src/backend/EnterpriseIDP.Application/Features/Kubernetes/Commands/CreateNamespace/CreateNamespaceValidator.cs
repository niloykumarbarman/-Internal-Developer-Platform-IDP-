using FluentValidation;

namespace EnterpriseIDP.Application.Features.Kubernetes.Commands.CreateNamespace;

public class CreateNamespaceValidator : AbstractValidator<CreateNamespaceCommand>
{
    public CreateNamespaceValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(63)
            .Matches(@"^[a-z0-9][a-z0-9\-]*[a-z0-9]$").WithMessage("Namespace must be lowercase alphanumeric with hyphens.");
        RuleFor(x => x.TeamId).NotEmpty();
    }
}
