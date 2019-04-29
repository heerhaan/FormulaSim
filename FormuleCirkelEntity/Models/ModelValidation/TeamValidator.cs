using FluentValidation;
using FormuleCirkelEntity.DAL;
using System.Linq;

namespace FormuleCirkelEntity.Models.ModelValidation
{
    public class TeamValidator : AbstractValidator<Team>
    {
        private readonly FormulaContext _context;

        public TeamValidator(FormulaContext context)
        {
            //Sets up database
            _context = context;

            RuleFor(t => t.Name).NotEmpty().WithMessage("Naam voor team is verplicht!");
            RuleFor(t => t.Abbreviation).NotEmpty().WithMessage("Afkorting is verplicht!")
                .Must(UniqueAbbreviation).WithMessage("Afkorting is al bezet!");
        }

        private bool UniqueAbbreviation(Team team, string abb)
        {
            var group = _context.Teams
                .Where(t => t.Abbreviation.ToUpper() == abb.ToUpper())
                .SingleOrDefault();

            if (group == null) return true;

            return team.TeamId == group.TeamId;
        }
    }
}
