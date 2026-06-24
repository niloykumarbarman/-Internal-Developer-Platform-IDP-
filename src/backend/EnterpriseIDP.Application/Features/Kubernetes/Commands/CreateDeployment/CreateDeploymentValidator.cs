using FluentValidation;

namespace EnterpriseIDP.Application.Features.Kubernetes.Commands.CreateDeployment;

public class CreateDeploymentValidator : AbstractValidator<CreateDeploymentCommand>
{
    public CreateDeploymentValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Deployment name is required.")
            .MaximumLength(100);

        RuleFor(x => x.ServiceId)
            .NotEmpty().WithMessage("ServiceId is required.");

        RuleFor(x => x.NamespaceId)
            .NotEmpty().WithMessage("NamespaceId is required.");

        RuleFor(x => x.ImageName)
            .NotEmpty().WithMessage("ImageName is required.");

        RuleFor(x => x.ImageTag)
            .NotEmpty().WithMessage("ImageTag is required.");

        RuleFor(x => x.Replicas)
            .InclusiveBetween(1, 50).WithMessage("Replicas must be between 1 and 50.");
    }
}
