using FluentValidation;
using FormuleCirkelEntity.DAL;
using Microsoft.EntityFrameworkCore;
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

            RuleFor(t => t.Name).NotEmpty().WithMessage("Name is required!");
            RuleFor(t => t.Abbreviation).NotEmpty().WithMessage("Abbreviation is required!")
                .Must(UniqueAbbreviation).WithMessage("Abbreviation is already used!");
        }

        private bool UniqueAbbreviation(Team team, string abb)
        {
            var group = _context.Teams.IgnoreQueryFilters()
                .Where(t => t.Abbreviation.ToUpper() == abb.ToUpper())
                .SingleOrDefault();

            if (group == null) return true;

            return team.Id == group.Id;
        }
    }
}