using System;

namespace ConsoleApplication3.Parsing
{
    public sealed class ArgumentParserException<TResult> : Exception
    {
        internal ArgumentParserException(string message, Command<TResult> command = null)
            : base(string.IsNullOrEmpty(command?.Name) ? message : $"{command.Name} - {message}") { }
    }
}