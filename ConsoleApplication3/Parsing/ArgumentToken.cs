using System;
using System.Diagnostics;
using System.Linq;

namespace ConsoleApplication3.Parsing
{
    [DebuggerDisplay("{ToString(), nq}")]
    internal struct ArgumentToken : IEquatable<ArgumentToken>
    {
        private ArgumentToken(string value, string modifier)
        {
            Value = value;
            Modifier = modifier;
        }

        public string Value { get; }

        public string Modifier { get; }

        public bool IsLiteral => string.IsNullOrEmpty(Modifier);

        private bool IsSwitch => Modifier == "-";

        public bool TryExplodeSwitchGroup(out ArgumentToken[] tokens)
        {
            if (IsSwitch && Value.Length > 1)
            {
                tokens = Value.Select(option => Option("-", option.ToString())).ToArray();
                return true;
            }

            tokens = null;
            return false;
        }

        public static ArgumentToken Literal(string value)
        {
            return new ArgumentToken(value, string.Empty);
        }

        public static ArgumentToken Option(string modifier, string name)
        {
            return new ArgumentToken(name, modifier);
        }

        public bool Equals(ArgumentToken other)
        {
            return string.Equals(Value, other.Value) && string.Equals(Modifier, other.Modifier);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is ArgumentToken && Equals((ArgumentToken) obj);
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