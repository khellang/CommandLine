using System;
using System.Diagnostics;
using System.Linq;

namespace ConsoleApplication3.Parsing
{
    [DebuggerDisplay("{ToString(), nq}")]
    internal struct ArgumentToken : IEquatable<ArgumentToken>
    {
        private ArgumentToken(StringComparer comparer, string value, string modifier)
        {
            Comparer = comparer;
            Value = value;
            Modifier = modifier;
        }

        public StringComparer Comparer { get; }

        public string Value { get; }

        public string Modifier { get; }

        public bool IsLiteral => string.IsNullOrEmpty(Modifier);

        private bool IsSwitch => Modifier == "-";

        public bool TryExplodeSwitchGroup(StringComparer comparer, out ArgumentToken[] tokens)
        {
            if (IsSwitch && Value.Length > 1)
            {
                tokens = Value.Select(option => Option(comparer, "-", option.ToString())).ToArray();
                return true;
            }

            tokens = null;
            return false;
        }

        public static ArgumentToken Literal(StringComparer comparer, string value)
        {
            return new ArgumentToken(comparer, value, string.Empty);
        }

        public static ArgumentToken Option(StringComparer comparer, string modifier, string name)
        {
            return new ArgumentToken(comparer, name, modifier);
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