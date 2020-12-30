using System;
using System.Collections.Generic;
using System.Linq;
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

        public static string ValidateListsEqualLength(int keyLen, int minLen, int maxLen)
        {
            string errString = "";
            if (keyLen != minLen || keyLen != maxLen)
                errString += "Given values aren't of the same size";
            return errString;
        }

        // Used to ensure that values in a list are in order, necessary for the list of keys for the development ranges
        public static string CheckIfListIsInOrder(IList<int> Range)
        {
            var orderedList = Range.OrderBy(a => a);
            if (!Range.SequenceEqual(orderedList))
                return "Keys aren't in order";
            else
                return "";
        }
    }
}
