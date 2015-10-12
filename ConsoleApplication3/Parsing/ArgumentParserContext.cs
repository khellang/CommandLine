using System.Collections.Generic;

namespace ConsoleApplication3.Parsing
{
    internal class ArgumentParserContext<TResult> : OptionParserContext<TResult>
    {
        public ArgumentParserContext(string commandName,
            IReadOnlyDictionary<string, MappedProperty<TResult>> options,
            Queue<MappedProperty<TResult>> arguments,
            Queue<ArgumentToken> tokens,
            object args,
            MappedProperty<TResult> property,
            List<string> values)
            : base(commandName, options, arguments, tokens, args, property)
        {
            Values = values;
        }

        public List<string> Values { get; }
    }
}