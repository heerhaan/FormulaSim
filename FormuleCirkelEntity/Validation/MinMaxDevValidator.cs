using System;
using FluentValidation;
using FluentValidation.Results;
using FormuleCirkelEntity.Models;

namespace FormuleCirkelEntity.Validation
{
    public class MinMaxDevValidator : AbstractValidator<MinMaxDevRange>
    {
        public MinMaxDevValidator()
        {
            RuleFor(min => min.MinDev).LessThan(max => max.MaxDev);
            RuleFor(max => max.MaxDev).LessThan(9999);
        }

        public static ValidationResult ValidateMinMax(MinMaxDevRange checkVal)
        {
            MinMaxDevValidator validator = new MinMaxDevValidator();
            return validator.Validate(checkVal);
        }
    }
}
