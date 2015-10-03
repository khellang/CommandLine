using System.Collections.Generic;
using ConsoleApplication3.Parsing;

namespace ConsoleApplication3.Model
{
    internal class ApplicationModel<TResult> : IApplicationModel<TResult>
    {
        public ApplicationModel(ApplicationConfiguration config, IReadOnlyDictionary<string, ICommandModel<TResult>> commands)
        {
            Config = config;
            Commands = commands;
        }

        private ApplicationConfiguration Config { get; }

        public IReadOnlyDictionary<string, ICommandModel<TResult>> Commands { get; }

        public TResult Run(string[] args)
        {
            var lexer = new ArgumentLexer(Config);

            var tokens = lexer.Lex(args);

            var parser = new ArgumentParser<TResult>(Config, Commands);

            var result = parser.Parse(tokens);

            return result.Command.Execute(result.Args);
        }
    }
}