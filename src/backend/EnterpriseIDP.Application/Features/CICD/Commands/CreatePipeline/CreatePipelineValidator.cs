using FluentValidation;

namespace EnterpriseIDP.Application.Features.CICD.Commands.CreatePipeline;

public class CreatePipelineValidator : AbstractValidator<CreatePipelineCommand>
{
    public CreatePipelineValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Pipeline name is required.")
            .MaximumLength(100);

        RuleFor(x => x.ServiceId)
            .NotEmpty().WithMessage("ServiceId is required.");

        RuleFor(x => x.RepositoryUrl)
            .NotEmpty().WithMessage("RepositoryUrl is required.")
            .Must(url => url.StartsWith("http://") || url.StartsWith("https://"))
            .WithMessage("RepositoryUrl must be a valid URL.");

        RuleFor(x => x.Branch)
            .NotEmpty().WithMessage("Branch is required.");
    }
}
