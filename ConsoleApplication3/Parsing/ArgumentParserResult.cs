﻿namespace ConsoleApplication3.Parsing
{
    internal class ArgumentParserResult<TResult>
    {
        public ArgumentParserResult(object args, ICommandModel<TResult> command)
        {
            Args = args;
            Command = command;
        }

        public object Args { get; }

        public ICommandModel<TResult> Command { get; }
    }
}