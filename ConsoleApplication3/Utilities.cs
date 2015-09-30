namespace ConsoleApplication3
{
    internal static class Utilities
    {
        public static bool IsValidName(string name)
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