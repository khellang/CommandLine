using System;

namespace ConsoleApplication3.Parsing
{
    public class ArgumentParserException : Exception
    {
        internal ArgumentParserException(string message, ICommandModel command = null)
            : base(string.IsNullOrEmpty(command?.Name) ? message : $"{command.Name} - {message}") { }
    }
}