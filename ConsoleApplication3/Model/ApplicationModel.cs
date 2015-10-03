using System.Collections.Generic;
using ConsoleApplication3.Parsing;

namespace ConsoleApplication3.Model
{
    internal class ApplicationModel<TResult> : IApplicationModel<TResult>
    {
        public ApplicationModel(IReadOnlyDictionary<string, ICommandModel<TResult>> commands)
        {
            Commands = commands;
        }

        public IReadOnlyDictionary<string, ICommandModel<TResult>> Commands { get; }

        public TResult Run(string[] args)
        {
            var tokens = ArgumentLexer.Lex(args);

            var parser = new ArgumentParser<TResult>(Commands);

            var result = parser.Parse(tokens);

            return result.Command.Execute(result.Args);
        }
    }
}