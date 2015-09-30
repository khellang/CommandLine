using System;
using System.Collections.Generic;

namespace ConsoleApplication3.Parsing
{
    internal class ArgumentParser<TResult>
    {
        public ArgumentParser(IReadOnlyDictionary<string, ICommandModel<TResult>> commands)
        {
            Commands = commands;
        }

        private IReadOnlyDictionary<string, ICommandModel<TResult>> Commands { get; }

        public ArgumentParserResult<TResult> Parse(IEnumerable<ArgumentToken> tokens)
        {
            var tokenQueue = new Queue<ArgumentToken>(tokens);

            var command = GetCommand(tokenQueue);

            var args = Activator.CreateInstance(command.Type);

            IOptionModel optionModel = null;

            while (tokenQueue.Count > 0)
            {
                var token = tokenQueue.Dequeue();

                switch (token.Kind)
                {
                    case ArgumentTokenKind.Literal:
                        if (optionModel == null)
                        {
                            // We don't support positional arguments yet.
                            throw new ArgumentParserException($"invalid argument '{token.Value}'", command);
                        }

                        if (optionModel.IsList())
                        {
                            var values = new List<string> { token.Value };

                            while (tokenQueue.Count > 0 && tokenQueue.Peek().IsLiteral)
                            {
                                values.Add(tokenQueue.Dequeue().Value);
                            }

                            optionModel.SetValues(args, values.ToArray());
                            optionModel = null;
                            break;
                        }

                        optionModel.SetValue(args, token.Value);
                        optionModel = null;
                        break;
                    case ArgumentTokenKind.Option:
                        if (optionModel != null)
                        {
                            // Two options right after each other.
                            throw new ArgumentParserException($"option '{optionModel.Name}' requires a value", command);
                        }

                        if (!command.Options.TryGetValue(token.Value, out optionModel))
                        {
                            throw new ArgumentParserException($"unknown option '{token.Value}'", command);
                        }

                        if (optionModel.IsFlag())
                        {
                            var flagValue = "true";

                            if (tokenQueue.Count > 0 && tokenQueue.Peek().IsLiteral)
                            {
                                // Explicit flag parameter, i.e. --quiet:true
                                flagValue = tokenQueue.Dequeue().Value;
                            }

                            optionModel.SetValue(args, flagValue);
                            optionModel = null;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(token.Kind));
                }
            }

            if (optionModel != null)
            {
                // The last token was an option, but we didn't received a value.
                throw new ArgumentParserException($"option '{optionModel.Name}' requires a value", command);
            }

            return new ArgumentParserResult<TResult>(args, command);
        }

        private ICommandModel<TResult> GetCommand(Queue<ArgumentToken> tokens)
        {
            if (tokens.Count == 0)
            {
                // TODO: Return help command.
                throw new ArgumentParserException("please specify command");
            }

            var commandToken = tokens.Dequeue();

            ICommandModel<TResult> command;
            if (!Commands.TryGetValue(commandToken.Value, out command))
            {
                throw new ArgumentParserException($"unknown command '{commandToken.Value}'");
            }

            return command;
        }
    }
}