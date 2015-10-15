using System;
using System.Collections.Generic;
using ConsoleApplication3.Extensions;

namespace ConsoleApplication3
{
    internal class CommandRegistry<TResult, TBuilder> : ICommandRegistry<TResult, TBuilder>
        where TBuilder : ICommandRegistry<TResult, TBuilder>, IConfigurable<TResult>
    {
        public CommandRegistry(TBuilder builder)
        {
            Builder = builder;
            Commands = new Dictionary<string, Command<TResult>>(Builder.Config.StringComparer);
        }

        public TBuilder Builder { get; }

        public Dictionary<string, Command<TResult>> Commands { get; }

        public TBuilder AddCommand<TArgs>(string name, Func<ICommandBuilder<TArgs, TResult>, Func<TArgs, TResult>> build)
        {
            var trimmed = name.Trim();

            if (!trimmed.IsValidName())
            {
                throw new FormatException($"The command name '{trimmed}' is invalid.");
            }

            var builder = new CommandBuilder<TArgs, TResult>(Builder.Config, trimmed);

            var execute = build(builder);

            var command = builder.Build(execute);

            Commands.Add(trimmed, command);

            return Builder;
        }
    }
}