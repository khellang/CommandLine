using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ConsoleApplication3.Model
{
    internal sealed class CommandModelBuilder<TArgs, TResult> : ICommandModelBuilder<TArgs, TResult>
        where TArgs : new()
    {
        public CommandModelBuilder(ApplicationConfiguration<TResult> config, string name)
        {
            Config = config;
            Name = name;
            Options = new Dictionary<string, IOptionModel<TResult>>(StringComparer.Ordinal);
        }

        private ApplicationConfiguration<TResult> Config { get; }

        private string Name { get; }

        private Dictionary<string, IOptionModel<TResult>> Options { get; }

        public ICommandModelBuilder<TArgs, TResult> MapOption<TProperty>(string option, Expression<Func<TArgs, TProperty>> mapping)
        {
            var options = OptionModel<TResult>.Create(Config, option, mapping);

            foreach (var optionModel in options)
            {
                Options.Add(optionModel.Key, optionModel.Value);
            }

            return this;
        }

        public ICommandModel<TResult> Build(Func<TArgs, TResult> execute)
        {
            return CommandModel<TResult>.Create(Name, Options, execute);
        }
    }
}