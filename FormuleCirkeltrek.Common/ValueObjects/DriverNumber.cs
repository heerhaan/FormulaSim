using System;
using System.Linq;

namespace FormuleCirkeltrek.Common.ValueObjects
{
    public struct DriverNumber : IEquatable<DriverNumber>, IEquatable<DriverNumber?>, IEquatable<short>, IEquatable<int>, IEquatable<long>
    {
        public DriverNumber(long number)
            : this(number.ToString()) { }

        public DriverNumber(int number)
            : this(number.ToString()) { }

        public DriverNumber(short number)
            : this(number.ToString()) { }

        public DriverNumber(string number)
        {
            if (number.Any(c => !char.IsDigit(c))
                   || number.Length > 2
                   || number.Length <= 0)
                throw new FormatException("DriverNumber can only consist of 1 or 2 digits.");

            if (number == "0" || number == "00")
                throw new FormatException("DriverNumber cannot be 0.");
            _number = number;
        }

        readonly string _number;

        #region Overrides / Implementations

        public override string ToString()
            => _number.ToString();

        public override bool Equals(object? obj)
            => Equals(obj as DriverNumber?);

        public override int GetHashCode()
            => _number.GetHashCode();

        public bool Equals(DriverNumber other)
            => _number.Equals(other._number);

        public bool Equals(DriverNumber? other)
            => _number.Equals(other?._number);

        public bool Equals(short other)
            => Equals(other.ToString());
        
        public bool Equals(int other)
            => Equals(other.ToString());
        
        public bool Equals(long other)
            => Equals(other.ToString());

        #endregion

        #region Implicit conversions

        public static implicit operator DriverNumber(string value)
            => new DriverNumber(value);

        public static implicit operator DriverNumber(short value)
            => new DriverNumber(value);

        public static implicit operator DriverNumber(int value)
            => new DriverNumber(value);

        public static implicit operator DriverNumber(long value)
            => new DriverNumber(value);

        #endregion

        #region Operator overloads

        public static bool operator ==(DriverNumber x, string y)
            => x.Equals(y);
        public static bool operator !=(DriverNumber x, string y)
            => !x.Equals(y);

        public static bool operator ==(DriverNumber x, short y)
            => x.Equals(y);
        public static bool operator !=(DriverNumber x, short y)
            => !x.Equals(y);

        public static bool operator ==(DriverNumber x, int y)
            => x.Equals(y);
        public static bool operator !=(DriverNumber x, int y)
            => !x.Equals(y);

        public static bool operator ==(DriverNumber x, long y)
            => x.Equals(y);
        public static bool operator !=(DriverNumber x, long y)
            => !x.Equals(y);

        #endregion
    }
}
