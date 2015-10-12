using System.Collections.Generic;
using ConsoleApplication3.Extensions;

namespace ConsoleApplication3.Parsing
{
    internal sealed class ArgumentParser<TResult>
    {
        public ArgumentParser(ApplicationConfiguration<TResult> config, IReadOnlyDictionary<string, Command<TResult>> commands)
        {
            Config = config;
            Commands = commands;
        }

        private ApplicationConfiguration<TResult> Config { get; }

        private IReadOnlyDictionary<string, Command<TResult>> Commands { get; }

        public ArgumentParserResult<TResult> Parse(IEnumerable<ArgumentToken> tokens)
        {
            var tokenQueue = new Queue<ArgumentToken>(tokens);

            var command = GetCommand(tokenQueue);

            var blankArgs = Config.ArgumentActivator(command.Type);

            var argumentQueue = new Queue<MappedProperty<TResult>>(command.Arguments);

            var context = ParseOptions(new ParserContext<TResult>(command.Name, command.Options, argumentQueue, tokenQueue, blankArgs));

            return new ArgumentParserResult<TResult>(context.Args, command);
        }

        private static ParserContext<TResult> ParseOptions(ParserContext<TResult> context)
        {
            if (context.Tokens.Count == 0)
            {
                // No options were specified.
                return context;
            }

            var token = context.Tokens.Peek();

            if (token.IsLiteral)
            {
                if (context.Arguments.Count == 0)
                {
                    throw new ArgumentParserException($"Invalid argument: {token.Value}", context.CommandName);
                }

                var argument = context.Arguments.Dequeue();

                return ParseArguments(context.WithOption(argument));
            }

            var optionToken = context.Tokens.Dequeue();

            MappedProperty<TResult> property;
            if (!context.Options.TryGetValue(optionToken.Value, out property))
            {
                throw new ArgumentParserException($"Unknown option: {optionToken.Value}", context.CommandName);
            }

            return ParseArguments(context.WithOption(property));
        }

        private static ParserContext<TResult> ParseArguments(OptionParserContext<TResult> context)
        {
            if (NextIsLiteral(context.Tokens))
            {
                var token = context.Tokens.Dequeue();

                if (context.Property.IsList())
                {
                    var values = new List<string> { token.Value };

                    // We're expecting multiple arguments, continue.
                    return ParseArgumentsList(context.WithValues(values));
                }

                context.Property.SetValue(context.Args, token.Value);

                // Ok, we have our value. Start looking for options again.
                return ParseOptions(context);
            }

            if (context.Property.IsFlag())
            {
                context.Property.SetValue(context.Args, "true");

                // There was no argument, but it's a flag, so we'll set it to true.
                return ParseOptions(context);
            }

            throw new ArgumentParserException($"Option '{context.Property.Name}' requires a value", context.CommandName);
        }

        private static ParserContext<TResult> ParseArgumentsList(ArgumentParserContext<TResult> context)
        {
            if (NextIsLiteral(context.Tokens))
            {
                var token = context.Tokens.Dequeue();

                context.Values.Add(token.Value);

                // Look for more arguments.
                return ParseArgumentsList(context);
            }

            context.Property.SetValues(context.Args, context.Values.ToArray());

            // We have our list arguments. Start looking for options again.
            return ParseOptions(context);
        }

        private static bool NextIsLiteral(Queue<ArgumentToken> tokens)
        {
            return tokens.Count > 0 && tokens.Peek().IsLiteral;
        }

        private Command<TResult> GetCommand(Queue<ArgumentToken> tokens)
        {
            if (tokens.Count == 0)
            {
                // TODO: No command was specified. Return help command.
                throw new ArgumentParserException("Please specify a command");
            }

            var commandToken = tokens.Dequeue();

            Command<TResult> command;
            if (!Commands.TryGetValue(commandToken.Value, out command))
            {
                throw new ArgumentParserException($"Unknown command: {commandToken.Value}");
            }

            return command;
        }
    }
}