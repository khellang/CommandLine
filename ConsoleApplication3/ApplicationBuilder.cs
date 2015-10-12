using System;
using System.Collections.Generic;
using ConsoleApplication3.Extensions;

namespace ConsoleApplication3
{
    internal sealed class ApplicationBuilder<TResult> : IApplicationBuilder<TResult>
    {
        public ApplicationBuilder(ApplicationConfiguration<TResult> config)
        {
            Config = config;
            Commands = new Dictionary<string, Command<TResult>>(config.StringComparer);
        }

        public ApplicationConfiguration<TResult> Config { get; }

        public Dictionary<string, Command<TResult>> Commands { get; }

        public IApplicationBuilder<TResult> AddCommand<TArgs>(string name, Func<ICommandBuilder<TArgs, TResult>, Func<TArgs, TResult>> build)
        {
            var trimmed = name.Trim();

            if (!trimmed.IsValidName())
            {
                throw new FormatException($"The command name '{trimmed}' is invalid.");
            }

            var builder = new CommandBuilder<TArgs, TResult>(Config, trimmed);

            var execute = build(builder);

            var command = builder.Build(execute);

            Commands.Add(trimmed, command);
            return this;
        }

        public Application<TResult> Build()
        {
            return new Application<TResult>(Config, Commands);
        }
    }
}