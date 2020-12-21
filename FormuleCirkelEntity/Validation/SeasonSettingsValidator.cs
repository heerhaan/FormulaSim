using FluentValidation;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.ViewModels;
using System;
using System.Linq;

namespace FormuleCirkelEntity.Validation
{
    public class SeasonSettingsValidator : AbstractValidator<SeasonSettingsViewModel>
    {
        public SeasonSettingsValidator()
        {
            RuleFor(x => x.QualificationRNG).InclusiveBetween(2, int.MaxValue);
            RuleFor(x => x.QualificationRemainingDriversQ2).InclusiveBetween(2, int.MaxValue);
            RuleFor(x => x.QualificationRemainingDriversQ3).InclusiveBetween(2, int.MaxValue);
            RuleFor(x => x.PitMin).LessThan(x => x.PitMax);
        }
    }
}
