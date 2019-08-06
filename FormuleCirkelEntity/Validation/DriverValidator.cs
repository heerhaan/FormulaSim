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
            RuleFor(d => d.DriverNumber).NotEmpty().WithMessage("Number of driver is required!");
            RuleFor(d => d.Name).NotEmpty().WithMessage("Name of driver is required!");
            RuleFor(d => d.Abbreviation).NotEmpty().WithMessage("Abbreviation is required!");
        }
    }
}