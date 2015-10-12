using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConsoleApplication3.Extensions
{
    internal static class StringExtensions
    {
        private const RegexOptions SplitterOptions = RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace;

        private readonly static Regex Splitter = new Regex(@"(?<=[a-z0-9])(?=[A-Z])|\s+", SplitterOptions);

        public static string KebabCase(this string value)
        {
            return string.Join("-", Splitter.Split(value.Trim()).Select(x => x.ToLower()));
        }

        public static bool IsValidName(this string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            if (name[0] == '-')
            {
                return false;
            }

            foreach (var @char in name)
            {
                if (!IsValidCharacter(@char))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsValidCharacter(char @char)
        {
            return char.IsLetterOrDigit(@char) || @char == '-' || @char == '_';
        }
    }
}