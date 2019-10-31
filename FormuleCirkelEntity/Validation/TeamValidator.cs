using FluentValidation;
using FormuleCirkelEntity.DAL;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FormuleCirkelEntity.Models.ModelValidation
{
    public class TeamValidator : AbstractValidator<Team>
    {
        public TeamValidator(FormulaContext context)
        {
            RuleFor(t => t.Name).NotEmpty().WithMessage("Name is required!");
            RuleFor(t => t.Abbreviation).NotEmpty().WithMessage("Abbreviation is required!");
        }
    }
}