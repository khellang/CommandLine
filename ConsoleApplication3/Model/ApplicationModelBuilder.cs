using System;
using System.Collections.Generic;

namespace ConsoleApplication3
{
    internal class ApplicationModelBuilder<TResult> : IApplicationModelBuilder<TResult>
    {
        public ApplicationModelBuilder(ApplicationConfiguration config)
        {
            Config = config;
            Commands = new Dictionary<string, ICommandModel<TResult>>(StringComparer.Ordinal);
        }

        private ApplicationConfiguration Config { get; }

        private Dictionary<string, ICommandModel<TResult>> Commands { get; }

        public IApplicationModelBuilder<TResult> AddCommand<TArgs>(string name, Func<ICommandModelBuilder<TArgs, TResult>, Func<TArgs, TResult>> build)
            where TArgs : new()
        {
            var builder = new CommandModelBuilder<TArgs, TResult>(Config, name);

            var execute = build(builder);

            var command = builder.Build(execute);

            Commands.Add(name, command);
            return this;
        }

        public ApplicationModel<TResult> Build()
        {
            return new ApplicationModel<TResult>(Commands);
        }
    }
}