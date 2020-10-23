using FluentValidation;
using FormuleCirkelEntity.DAL;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FormuleCirkelEntity.Models.ModelValidation
{
    public class TeamValidator : AbstractValidator<Team>
    {
        public TeamValidator()
        {
            RuleFor(t => t.Abbreviation).NotEmpty().WithMessage("Abbreviation is required!");
        }
    }
}