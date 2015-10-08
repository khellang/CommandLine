using System;
using System.Collections.Generic;
using ConsoleApplication3.Model;
using ConsoleApplication3.Parsing;

namespace ConsoleApplication3
{
    public static class Application
    {
        public static TResult Run<TResult>(ApplicationConfiguration<TResult> config, string[] args, Action<IApplicationBuilder<TResult>> build)
        {
            return Create(config, build).Run(args);
        }

        internal static Application<TResult> Create<TResult>(ApplicationConfiguration<TResult> config, Action<IApplicationBuilder<TResult>> build)
        {
            var builder = new ApplicationBuilder<TResult>(config);

            build(builder);

            return builder.Build();
        }
    }

    internal class Application<TResult>
    {
        public Application(ApplicationConfiguration<TResult> config, IReadOnlyDictionary<string, Command<TResult>> commands)
        {
            Config = config;
            Commands = commands;
        }

        private ApplicationConfiguration<TResult> Config { get; }

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