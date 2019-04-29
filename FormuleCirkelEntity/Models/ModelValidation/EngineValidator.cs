using FluentValidation;
using FormuleCirkelEntity.DAL;
using System.Linq;

namespace FormuleCirkelEntity.Models.ModelValidation
{
    public class EngineValidator : AbstractValidator<Engine>
    {
        private readonly FormulaContext _context;

        public EngineValidator(FormulaContext context)
        {
            //Sets up database
            _context = context;

            RuleFor(e => e.Name).NotEmpty().WithMessage("Naam is verplicht!")
                .Must(UniqueName).WithMessage("Naam is al bezet!");
            RuleFor(e => e.Power).NotEmpty().WithMessage("Motor kracht is verplicht!");
        }

        private bool UniqueName(Engine engine, string name)
        {
            var group = _context.Engines
                .Where(t => t.Name.ToUpper() == name.ToUpper())
                .SingleOrDefault();

            if (group == null) return true;

            return engine.EngineId == group.EngineId;
        }
    }
}