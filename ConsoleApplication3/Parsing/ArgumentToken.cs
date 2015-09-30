using System;
using System.Diagnostics;
using System.Linq;

namespace ConsoleApplication3.Parsing
{
    [DebuggerDisplay("{ToString(), nq}")]
    internal struct ArgumentToken : IEquatable<ArgumentToken>
    {
        private ArgumentToken(ArgumentTokenKind kind, string value, string modifier)
        {
            Kind = kind;
            Value = value;
            Modifier = modifier;
        }

        public ArgumentTokenKind Kind { get; }

        public string Value { get; }

        public string Modifier { get; }

        public bool IsLiteral => Kind == ArgumentTokenKind.Literal;

        public bool IsOption => Kind == ArgumentTokenKind.Option;

        private bool IsSwitch => Modifier == "-";

        public bool TryExplodeSwitchGroup(out ArgumentToken[] tokens)
        {
            if (IsOption && IsSwitch && Value.Length > 1)
            {
                tokens = Value.Select(option => Option("-", option.ToString())).ToArray();
                return true;
            }

            tokens = null;
            return false;
        }

        public static ArgumentToken Literal(string value)
        {
            return new ArgumentToken(ArgumentTokenKind.Literal, value, string.Empty);
        }

        public static ArgumentToken Option(string modifier, string name)
        {
            return new ArgumentToken(ArgumentTokenKind.Option, name, modifier);
        }

        public bool Equals(ArgumentToken other)
        {
            return Kind == other.Kind && string.Equals(Value, other.Value) && string.Equals(Modifier, other.Modifier);
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
                var hashCode = (int) Kind;
                hashCode = (hashCode * 397) ^ Value.GetHashCode();
                hashCode = (hashCode * 397) ^ Modifier.GetHashCode();
                return hashCode;
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
            return $"{Kind}: {Modifier}{Value}";
        }
    }
}