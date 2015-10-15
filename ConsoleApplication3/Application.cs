using System;
using System.Collections.Generic;
using ConsoleApplication3.Parsing;

namespace ConsoleApplication3
{
    public static class Application
    {
        public static TResult Run<TResult>(Configuration<TResult> config, string[] args, Action<IApplicationBuilder<TResult>> build)
        {
            return Create(config, build).Run(args);
        }

        public static TResult Run<TArgs, TResult>(Configuration<TResult> config, string[] args, Func<IApplicationBuilder<TArgs, TResult>, Func<TArgs, TResult>> build)
        {
            return Create(config, build).Run(args);
        }

        internal static Application<TResult> Create<TResult>(Configuration<TResult> config, Action<IApplicationBuilder<TResult>> build)
        {
            var builder = new ApplicationBuilder<TResult>(config);

            build(builder);

            return builder.Build();
        }

        internal static Application<TResult> Create<TArgs, TResult>(Configuration<TResult> config, Func<IApplicationBuilder<TArgs, TResult>, Func<TArgs, TResult>> build)
        {
            var builder = new ApplicationBuilder<TArgs, TResult>(config);

            var execute = build(builder);

            return builder.Build(execute);
        }
    }

    internal class Application<TResult>
    {
        public Application(Configuration<TResult> config, IReadOnlyDictionary<string, Command<TResult>> commands)
        {
            Config = config;
            Commands = commands;
        }

        private Configuration<TResult> Config { get; }

        public IReadOnlyDictionary<string, Command<TResult>> Commands { get; }

        public TResult Run(string[] args)
        {
            try
            {
                var lexer = new ArgumentLexer<TResult>(Config);

                var tokens = lexer.Lex(args);

                var parser = new ArgumentParser<TResult>(Config, Commands);

                var result = parser.Parse(tokens);

                return result.Command.Execute(result.Args);
            }
            catch (Exception ex)
            {
                return Config.ErrorHandler(ex);
            }
        }
    }
}