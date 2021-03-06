﻿using System;

namespace ConsoleApplication3.Parsing
{
    public class ArgumentParserException : Exception
    {
        internal ArgumentParserException(string message, string commandName = null)
            : base(string.IsNullOrEmpty(commandName) ? message : $"{commandName} - {message}") { }
    }
}