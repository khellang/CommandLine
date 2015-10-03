using System;
using System.Collections.Generic;
using ConsoleApplication3.Parsing;

namespace ConsoleApplication3.Model
{
    internal sealed class ApplicationModel<TResult> : IApplicationModel<TResult>
    {
        public ApplicationModel(ApplicationConfiguration<TResult> config, IReadOnlyDictionary<string, ICommandModel<TResult>> commands)
        {
            Config = config;
            Commands = commands;
        }

        private ApplicationConfiguration<TResult> Config { get; }

        public IReadOnlyDictionary<string, ICommandModel<TResult>> Commands { get; }

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