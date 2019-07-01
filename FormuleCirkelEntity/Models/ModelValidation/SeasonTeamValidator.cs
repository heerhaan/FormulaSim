using FluentValidation;

namespace FormuleCirkelEntity.Models.ModelValidation
{
    public class SeasonTeamValidator : AbstractValidator<SeasonTeam>
    {
        public SeasonTeamValidator()
        {
            RuleFor(t => t.Principal).NotEmpty().WithMessage("Team principal is required!");
            RuleFor(t => t.Chassis).NotEmpty().WithMessage("Chassis value is required!");
            RuleFor(t => t.Reliability).NotEmpty().WithMessage("Reliability value is required!");
        }
    }
}