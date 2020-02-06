using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormuleCirkeltrek.Domain.ValueObjects
{
    public struct DriverNumber
    {
        public DriverNumber(short number)
            : this(number.ToString())
        {}

        public DriverNumber(string number)
        {
            _number = string.Empty;
            TryAssign(this, number);
        }

        string _number;

        static void TryAssign(DriverNumber instance, string number)
        {
            if (number.Any(c => !char.IsDigit(c))
                || number.Length > 2
                || number.Length <= 0)
                throw new FormatException("DriverNumber can only consist of 1 or 2 digits.");

            if (number == "0" || number == "00")
                throw new FormatException("DriverNumber cannot be 0.");
            instance._number = number;
        }
    }
}
