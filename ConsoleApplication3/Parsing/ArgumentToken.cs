using System;
using System.Diagnostics;
using System.Linq;

namespace ConsoleApplication3.Parsing
{
    [DebuggerDisplay("{ToString(), nq}")]
    internal struct ArgumentToken : IEquatable<ArgumentToken>
    {
        private ArgumentToken(string value, string modifier, StringComparer comparer)
        {
            Value = value;
            Modifier = modifier;
            Comparer = comparer;
        }

        public string Value { get; }

        public string Modifier { get; }

        public StringComparer Comparer { get; }

        public bool IsLiteral => string.IsNullOrEmpty(Modifier);

        public bool IsSwitch => Modifier == "-";

        public static ArgumentToken Literal(string value, StringComparer comparer)
        {
            return new ArgumentToken(value, string.Empty, comparer);
        }

        public static ArgumentToken Option(string modifier, string name, StringComparer comparer)
        {
            return new ArgumentToken(name, modifier, comparer);
        }

        public bool Equals(ArgumentToken other)
        {
            return Comparer.Equals(Value, other.Value) && Comparer.Equals(Modifier, other.Modifier);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is ArgumentToken && Equals((ArgumentToken) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Value.GetHashCode() * 397) ^ Modifier.GetHashCode();
            }
        }

        public static bool operator ==(ArgumentToken left, ArgumentToken right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ArgumentToken left, ArgumentToken right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"{(IsLiteral ? "Literal" : "Option")}: {Modifier}{Value}";
        }
    }
}