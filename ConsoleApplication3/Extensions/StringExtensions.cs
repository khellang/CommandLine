namespace ConsoleApplication3.Extensions
{
    internal static class StringExtensions
    {
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