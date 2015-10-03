using System;
using System.Collections.Generic;
using ConsoleApplication3.Extensions;
using ConsoleApplication3.Model;

namespace ConsoleApplication3.Parsing
{
    internal sealed class ArgumentParser<TResult>
    {
        public ArgumentParser(ApplicationConfiguration<TResult> config, IReadOnlyDictionary<string, ICommandModel<TResult>> commands)
        {
            Config = config;
            Commands = commands;
        }

        private ApplicationConfiguration<TResult> Config { get; }

        private IReadOnlyDictionary<string, ICommandModel<TResult>> Commands { get; }

        public ArgumentParserResult<TResult> Parse(IEnumerable<ArgumentToken> tokens)
        {
            var tokenQueue = new Queue<ArgumentToken>(tokens);

            var command = GetCommand(tokenQueue);

            var args = ParseOptions(tokenQueue, command, Activator.CreateInstance(command.Type));

            return new ArgumentParserResult<TResult>(args, command);
        }

        private static object ParseOptions(Queue<ArgumentToken> tokens, ICommandModel<TResult> command, object args)
        {
            if (tokens.Count == 0)
            {
                return args;
            }

            var token = tokens.Dequeue();

            if (token.IsLiteral)
            {
                throw new ArgumentParserException($"Invalid argument: {token.Value}", command);
            }

            IOptionModel<TResult> option;
            if (!command.Options.TryGetValue(token.Value, out option))
            {
                throw new ArgumentParserException($"Unknown option: {token.Value}", command);
            }

            return ParseArguments(tokens, command, args, option);
        }

        private static object ParseArguments(Queue<ArgumentToken> tokens, ICommandModel<TResult> command, object args, IOptionModel<TResult> option)
        {
            if (IsNextArgument(tokens))
            {
                var token = tokens.Dequeue();

                if (option.IsList())
                {
                    var values = new List<string> { token.Value };

                    return ParseListArgument(tokens, command, args, option, values);
                }

                option.SetValue(args, token.Value);

                return ParseOptions(tokens, command, args);
            }

            if (option.IsFlag())
            {
                option.SetValue(args, "true");

                return ParseOptions(tokens, command, args);
            }

            throw new ArgumentParserException($"Option '{option.Name}' requires a value", command);
        }

        private static object ParseListArgument(Queue<ArgumentToken> tokens, ICommandModel<TResult> command, object args, IOptionModel<TResult> option, List<string> values)
        {
            if (IsNextArgument(tokens))
            {
                var token = tokens.Dequeue();

                values.Add(token.Value);

                return ParseListArgument(tokens, command, args, option, values);
            }

            option.SetValues(args, values.ToArray());

            return ParseOptions(tokens, command, args);
        }

        private static bool IsNextArgument(Queue<ArgumentToken> tokens)
        {
            return tokens.Count > 0 && tokens.Peek().IsLiteral;
        }

        private ICommandModel<TResult> GetCommand(Queue<ArgumentToken> tokens)
        {
            if (tokens.Count == 0)
            {
                // TODO: Return help command.
                throw new ArgumentParserException("Please specify a command");
            }

            var commandToken = tokens.Dequeue();

            ICommandModel<TResult> command;
            if (!Commands.TryGetValue(commandToken.Value, out command))
            {
                throw new ArgumentParserException($"Unknown command: {commandToken.Value}");
            }

            return command;
        }
    }
}