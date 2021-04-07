using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using FormuleCirkelEntity.Models;

namespace FormuleCirkelEntity.Validation
{
    public class AppConfigValidator : AbstractValidator<AppConfig>
    {
        public AppConfigValidator()
        {
            RuleFor(res => res.MistakeLowerValue).LessThanOrEqualTo(res => res.MistakeUpperValue);
            RuleFor(res => res.DisqualifyChance).InclusiveBetween(0, 100);
            RuleFor(res => res.SunnyEngineMultiplier).GreaterThanOrEqualTo(0);
            RuleFor(res => res.OvercastEngineMultiplier).GreaterThanOrEqualTo(0);
            RuleFor(res => res.WetEngineMultiplier).GreaterThanOrEqualTo(0);
            RuleFor(res => res.MistakeAmountRolls).InclusiveBetween(0,10);
        }
    }
}
