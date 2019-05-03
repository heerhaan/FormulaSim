using FluentValidation;

namespace FormuleCirkelEntity.Models.ModelValidation
{
    public class SeasonDriverValidator : AbstractValidator<SeasonDriver>
    {
        public SeasonDriverValidator()
        {
            RuleFor(d => d.Skill).NotEmpty().WithMessage("Vaardigheid is verplicht!");
        }
    }
}