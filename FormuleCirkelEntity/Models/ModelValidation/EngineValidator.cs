using FluentValidation;
using FormuleCirkelEntity.DAL;
using System.Linq;

namespace FormuleCirkelEntity.Models.ModelValidation
{
    public class EngineValidator : AbstractValidator<Engine>
    {
        public EngineValidator(FormulaContext context)
        {
            RuleFor(e => e.Name).NotEmpty().WithMessage("Naam is verplicht!");
            RuleFor(e => e.Power).NotEmpty().WithMessage("Motor kracht is verplicht!");
        }
    }
}