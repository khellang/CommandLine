using System;
using ConsoleApplication3.Model;

namespace ConsoleApplication3.Parsing
{
    public sealed class ArgumentParserException : Exception
    {
        internal ArgumentParserException(string message, ICommandModel command = null)
            : base(string.IsNullOrEmpty(command?.Name) ? message : $"{command.Name} - {message}") { }
    }
}