using System.Collections.Generic;

namespace ConsoleApplication3.Parsing
{
    internal class ParserContext<TResult>
    {
        public ParserContext(string commandName,
            IReadOnlyDictionary<string, MappedProperty<TResult>> options,
            Queue<MappedProperty<TResult>> arguments,
            Queue<ArgumentToken> tokens,
            object args)
        {
            CommandName = commandName;
            Options = options;
            Arguments = arguments;
            Tokens = tokens;
            Args = args;
        }

        public string CommandName { get; }

        public IReadOnlyDictionary<string, MappedProperty<TResult>> Options { get; }

        public Queue<MappedProperty<TResult>> Arguments { get; }

        public Queue<ArgumentToken> Tokens { get; }

        public object Args { get; }

        public OptionParserContext<TResult> WithOption(MappedProperty<TResult> property)
        {
            return new OptionParserContext<TResult>(CommandName, Options, Arguments, Tokens, Args, property);
        }
    }
}