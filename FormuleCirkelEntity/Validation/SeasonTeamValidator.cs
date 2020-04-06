using FluentValidation;

namespace FormuleCirkelEntity.Models.ModelValidation
{
    public class SeasonTeamValidator : AbstractValidator<SeasonTeam>
    {
        public SeasonTeamValidator()
        {
            RuleFor(t => t.Name).NotEmpty().WithMessage("Name is required!");
            RuleFor(t => t.Principal).NotEmpty().WithMessage("Team principal is required!");
            RuleFor(t => t.Colour).NotEmpty().WithMessage("Colour is required!");
            RuleFor(t => t.Accent).NotEmpty().WithMessage("Accent is required!");
            RuleFor(t => t.Chassis).NotEmpty().WithMessage("Chassis value is required!");
            RuleFor(t => t.Reliability).NotEmpty().WithMessage("Reliability value is required!");
        }
    }
}