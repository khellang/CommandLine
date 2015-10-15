using System;
using System.Linq.Expressions;
namespace ConsoleApplication3
{
    internal class ApplicationBuilder<TResult> : IApplicationBuilder<TResult>
    {
        public ApplicationBuilder(ApplicationConfiguration<TResult> config)
        {
            Config = config;
            CommandRegistry = new CommandRegistry<TResult, IApplicationBuilder<TResult>>(this);
        }

        public ApplicationConfiguration<TResult> Config { get; }

        public CommandRegistry<TResult, IApplicationBuilder<TResult>> CommandRegistry { get; }

        public IApplicationBuilder<TResult> AddCommand<TArgs>(string name, Func<ICommandBuilder<TArgs, TResult>, Func<TArgs, TResult>> build)
        {
            return CommandRegistry.AddCommand(name, build);
        }

        public Application<TResult> Build()
        {
            return new Application<TResult>(Config, CommandRegistry.Commands);
        }
    }

    internal class ApplicationBuilder<TAppArgs, TResult> : IApplicationBuilder<TAppArgs, TResult>
    {
        public ApplicationBuilder(ApplicationConfiguration<TResult> config)
        {
            Config = config;
            CommandRegistry = new CommandRegistry<TResult, IApplicationBuilder<TAppArgs, TResult>>(this);
            OptionRegistry = new OptionRegistry<TAppArgs, TResult, IApplicationBuilder<TAppArgs, TResult>>(this);
            ArgumentRegistry = new ArgumentRegistry<TAppArgs, TResult, IApplicationBuilder<TAppArgs, TResult>>(this);
        }

        public ApplicationConfiguration<TResult> Config { get; }

        public CommandRegistry<TResult, IApplicationBuilder<TAppArgs, TResult>> CommandRegistry { get; }

        public OptionRegistry<TAppArgs, TResult, IApplicationBuilder<TAppArgs, TResult>> OptionRegistry { get; }

        public ArgumentRegistry<TAppArgs, TResult, IApplicationBuilder<TAppArgs, TResult>> ArgumentRegistry { get; }

        public IApplicationBuilder<TAppArgs, TResult> AddCommand<TArgs>(string name, Func<ICommandBuilder<TArgs, TResult>, Func<TArgs, TResult>> build)
        {
            return CommandRegistry.AddCommand(name, build);
        }

        public IApplicationBuilder<TAppArgs, TResult> AddOption<TProperty>(string names, Expression<Func<TAppArgs, TProperty>> mapping)
        {
            return OptionRegistry.AddOption(names, mapping);
        }

        public IApplicationBuilder<TAppArgs, TResult> AddArgument<TProperty>(string name, Expression<Func<TAppArgs, TProperty>> mapping)
        {
            return ArgumentRegistry.AddArgument(name, mapping);
        }

        public Application<TResult> Build(Func<TAppArgs, TResult> execute)
        {
            var command = new Command<TResult>(typeof(TAppArgs),
                string.Empty,
                args => execute((TAppArgs) args),
                OptionRegistry.Options,
                ArgumentRegistry.Arguments);

            CommandRegistry.Commands.Add(string.Empty, command);

            return new Application<TResult>(Config, CommandRegistry.Commands);
        }
    }
}