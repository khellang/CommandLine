using System.Collections.Generic;
using ConsoleApplication3.Extensions;

namespace ConsoleApplication3.Parsing
{
    internal sealed class ArgumentParser<TResult>
    {
        public ArgumentParser(ApplicationConfiguration<TResult> config,
            IReadOnlyDictionary<string, Command<TResult>> commands)
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

            var parsedArgs = ParseOptions(tokenQueue, command, blankArgs);

            return new ArgumentParserResult<TResult>(parsedArgs, command);
        }

        private static object ParseOptions(Queue<ArgumentToken> tokens, Command<TResult> command, object args)
        {
            if (tokens.Count == 0)
            {
                // No options were specified.
                return args;
            }

            var token = tokens.Dequeue();

            if (token.IsLiteral)
            {
                // We expected an option, but found an argument, since
                // we don't support positional arguments yet, we just throw.
                throw new ArgumentParserException<TResult>($"Invalid argument: {token.Value}", command);
            }

            Option<TResult> option;
            if (!command.Options.TryGetValue(token.Value, out option))
            {
                throw new ArgumentParserException<TResult>($"Unknown option: {token.Value}", command);
            }

            return ParseArguments(tokens, command, args, option);
        }

        private static object ParseArguments(Queue<ArgumentToken> tokens, Command<TResult> command, object args, Option<TResult> option)
        {
            if (NextIsLiteral(tokens))
            {
                var token = tokens.Dequeue();

                if (option.IsList())
                {
                    var values = new List<string> { token.Value };

                    // We're expecting multiple arguments, continue.
                    return ParseListArguments(tokens, command, args, option, values);
                }

                option.SetValue(args, token.Value);

                // Ok, we have our value. Start looking for options again.
                return ParseOptions(tokens, command, args);
            }

            if (option.IsFlag())
            {
                option.SetValue(args, "true");

                // There was not argument, but it's a flag, so we'll set it to true.
                return ParseOptions(tokens, command, args);
            }

            throw new ArgumentParserException<TResult>($"Option '{option.Name}' requires a value", command);
        }

        private static object ParseListArguments(Queue<ArgumentToken> tokens, Command<TResult> command, object args, Option<TResult> option, List<string> values)
        {
            if (NextIsLiteral(tokens))
            {
                var token = tokens.Dequeue();

                values.Add(token.Value);

                // Look for more arguments.
                return ParseListArguments(tokens, command, args, option, values);
            }

            option.SetValues(args, values.ToArray());

            // We have our list arguments. Start looking for options again.
            return ParseOptions(tokens, command, args);
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
                throw new ArgumentParserException<TResult>("Please specify a command");
            }

            var commandToken = tokens.Dequeue();

            Command<TResult> command;
            if (!Commands.TryGetValue(commandToken.Value, out command))
            {
                throw new ArgumentParserException<TResult>($"Unknown command: {commandToken.Value}");
            }

            return command;
        }
    }
}