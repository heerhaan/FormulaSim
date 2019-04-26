using FluentValidation;

namespace FormuleCirkelEntity.Models.ModelValidation
{
    public class SeasonTeamValidator : AbstractValidator<SeasonTeam>
    {
        public SeasonTeamValidator()
        {
            RuleFor(t => t.Chassis).NotEmpty().WithMessage("Chassis invoer is verplicht!");
            RuleFor(t => t.Reliability).NotEmpty().WithMessage("Betrouwbaarheid invoer is verplicht!");
        }
    }
}
