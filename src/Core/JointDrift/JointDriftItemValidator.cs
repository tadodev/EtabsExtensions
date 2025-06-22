using FluentValidation;
using EtabsExtensions.Domain.Entities.JointDrift;

namespace EtabsExtensions.Core.JointDrift.Validators;

public class JointDriftItemValidator : AbstractValidator<JointDriftItem>
{
    public JointDriftItemValidator()
    {
        RuleFor(x => x.Label).GreaterThan(0);
        RuleFor(x => x.StepNumber).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Story).NotEmpty();
        RuleFor(x => x.OutputCase).NotEmpty();
    }
}
