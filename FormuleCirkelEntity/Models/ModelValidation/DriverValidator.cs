using FluentValidation;
using FormuleCirkelEntity.DAL;
using System.Linq;

namespace FormuleCirkelEntity.Models.ModelValidation
{
    public class DriverValidator : AbstractValidator<Driver>
    {
        private readonly FormulaContext _context;

        public DriverValidator(FormulaContext context)
        {
            //Sets up database
            _context = context;

            RuleFor(d => d.DriverNumber).NotEmpty().WithMessage("Coureurnummer mag niet leeg zijn!");
            RuleFor(d => d.Name).NotEmpty().WithMessage("Naam coureur is verplicht!");
            RuleFor(d => d.Abbreviation).NotEmpty().WithMessage("Afkorting is verplicht!")
                .Must(UniqueAbbreviation).WithMessage("Afkorting is al bezet!");
        }

        private bool UniqueAbbreviation(Driver driver, string abb)
        {
            var group = _context.Drivers
                .Where(d => d.Abbreviation.ToUpper() == abb.ToUpper())
                .SingleOrDefault();

            if (group == null) return true;

            return driver.DriverId == group.DriverId;
        }
    }
}