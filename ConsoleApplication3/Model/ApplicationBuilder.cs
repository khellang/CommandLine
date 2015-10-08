using System;
using System.Collections.Generic;

namespace ConsoleApplication3.Model
{
    internal sealed class ApplicationBuilder<TResult> : IApplicationBuilder<TResult>
    {
        public ApplicationBuilder(ApplicationConfiguration<TResult> config)
        {
            Config = config;
            Commands = new Dictionary<string, Command<TResult>>(config.StringComparer);
        }

        private ApplicationConfiguration<TResult> Config { get; }

        private Dictionary<string, Command<TResult>> Commands { get; }

        public IApplicationBuilder<TResult> AddCommand<TArgs>(string name, Func<ICommandBuilder<TArgs, TResult>, Func<TArgs, TResult>> build)
        {
            var builder = new CommandBuilder<TArgs, TResult>(Config, name);

            var execute = build(builder);

            var command = builder.Build(execute);

            Commands.Add(name, command);
            return this;
        }

        public Application<TResult> Build()
        {
            return new Application<TResult>(Config, Commands);
        }
    }
}