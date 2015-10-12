using System.Collections.Generic;

namespace ConsoleApplication3.Parsing
{
    internal class OptionParserContext<TResult> : ParserContext<TResult>
    {
        public OptionParserContext(string commandName,
            IReadOnlyDictionary<string, MappedProperty<TResult>> options,
            Queue<MappedProperty<TResult>> arguments,
            Queue<ArgumentToken> tokens,
            object args,
            MappedProperty<TResult> property)
            : base(commandName, options, arguments, tokens, args)
        {
            Property = property;
        }

        public MappedProperty<TResult> Property { get; }

        public ArgumentParserContext<TResult> WithValues(List<string> values)
        {
            return new ArgumentParserContext<TResult>(CommandName, Options, Arguments, Tokens, Args, Property, values);
        }
    }
}