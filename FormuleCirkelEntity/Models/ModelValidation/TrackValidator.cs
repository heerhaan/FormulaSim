using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FormuleCirkelEntity.DAL;

namespace FormuleCirkelEntity.Models.ModelValidation
{
    public class TrackValidator : AbstractValidator<Track>
    {
        public TrackValidator()
        {
            RuleFor(t => t.Name).NotEmpty().WithMessage("Naam circuit is verplicht!");
            RuleFor(t => t.Location).NotEmpty().WithMessage("Locatie voor circuit is verplicht!");
        }
    }
}
