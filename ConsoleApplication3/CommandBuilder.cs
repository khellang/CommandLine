using System;
using System.Linq.Expressions;

namespace ConsoleApplication3
{
    internal class CommandBuilder<TArgs, TResult> : ICommandBuilder<TArgs, TResult>
    {
        public CommandBuilder(Configuration<TResult> config, string name)
        {
            Config = config;
            Name = name;
            OptionRegistry = new OptionRegistry<TArgs, TResult, ICommandBuilder<TArgs, TResult>>(this);
            ArgumentRegistry = new ArgumentRegistry<TArgs, TResult, ICommandBuilder<TArgs, TResult>>(this);
        }

        public Configuration<TResult> Config { get; }

        public string Name { get; }

        public OptionRegistry<TArgs, TResult, ICommandBuilder<TArgs, TResult>> OptionRegistry { get; }

        public ArgumentRegistry<TArgs, TResult, ICommandBuilder<TArgs, TResult>> ArgumentRegistry { get; }

        public ICommandBuilder<TArgs, TResult> Option<TProperty>(string names, Expression<Func<TArgs, TProperty>> mapping)
        {
            return OptionRegistry.Option(names, mapping);
        }

        public ICommandBuilder<TArgs, TResult> Argument<TProperty>(string name, Expression<Func<TArgs, TProperty>> mapping)
        {
            return ArgumentRegistry.Argument(name, mapping);
        }

        public Command<TResult> Build(Func<TArgs, TResult> execute)
        {
            return new Command<TResult>(typeof(TArgs), Name, args => execute((TArgs) args), OptionRegistry.Options, ArgumentRegistry.Arguments);
        }
    }
}