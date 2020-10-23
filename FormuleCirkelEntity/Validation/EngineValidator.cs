using FluentValidation;
using FormuleCirkelEntity.DAL;

namespace FormuleCirkelEntity.Models.ModelValidation
{
    public class EngineValidator : AbstractValidator<Engine>
    {
        public EngineValidator()
        {
            RuleFor(e => e.Name).NotEmpty().WithMessage("Name is required!");
            RuleFor(e => e.Power).NotEmpty().WithMessage("Engine power is required!");
        }
    }
}