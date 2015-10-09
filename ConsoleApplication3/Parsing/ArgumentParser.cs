using System;
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

            var context = ParseOptions(new ParserContext(tokenQueue, command, blankArgs));

            return new ArgumentParserResult<TResult>(context.Args, command);
        }

        private static ParserContext ParseOptions(ParserContext context)
        {
            if (context.Tokens.Count == 0)
            {
                // No options were specified.
                return context;
            }

            var token = context.Tokens.Dequeue();

            if (token.IsLiteral)
            {
                throw new ArgumentParserException<TResult>($"Invalid argument: {token.Value}", context.Command);
            }

            Option<TResult> option;
            if (!context.Command.Options.TryGetValue(token.Value, out option))
            {
                throw new ArgumentParserException<TResult>($"Unknown option: {token.Value}", context.Command);
            }

            return ParseOptionArguments(context.WithOption(option));
        }

        private static ParserContext ParseOptionArguments(OptionParserContext context)
        {
            if (NextIsLiteral(context.Tokens))
            {
                var token = context.Tokens.Dequeue();

                if (context.Option.IsList())
                {
                    var values = new List<string> { token.Value };

                    // We're expecting multiple arguments, continue.
                    return ParseOptionArgumentList(context.WithValues(values));
                }

                context.Option.SetValue(context.Args, token.Value);

                // Ok, we have our value. Start looking for options again.
                return ParseOptions(context);
            }

            if (context.Option.IsFlag())
            {
                context.Option.SetValue(context.Args, "true");

                // There was no argument, but it's a flag, so we'll set it to true.
                return ParseOptions(context);
            }

            throw new ArgumentParserException<TResult>($"Option '{context.Option.Name}' requires a value", context.Command);
        }

        private static ParserContext ParseOptionArgumentList(ArgumentParserContext context)
        {
            if (NextIsLiteral(context.Tokens))
            {
                var token = context.Tokens.Dequeue();

                context.Values.Add(token.Value);

                // Look for more arguments.
                return ParseOptionArgumentList(context);
            }

            context.Option.SetValues(context.Args, context.Values.ToArray());

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

        private class ParserContext
        {
            public ParserContext(Queue<ArgumentToken> tokens, Command<TResult> command, object args)
            {
                Tokens = tokens;
                Command = command;
                Args = args;
            }

            public Queue<ArgumentToken> Tokens { get; }

            public Command<TResult> Command { get;}

            public object Args { get; }

            public OptionParserContext WithOption(Option<TResult> option)
            {
                return new OptionParserContext(Tokens, Command, Args, option);
            }
        }

        private class OptionParserContext : ParserContext
        {
            public OptionParserContext(Queue<ArgumentToken> tokens, Command<TResult> command, object args, Option<TResult> option)
                : base(tokens, command, args)
            {
                Option = option;
            }

            public Option<TResult> Option { get; }

            public ArgumentParserContext WithValues(List<string> values)
            {
                return new ArgumentParserContext(Tokens, Command, Args, Option, values);
            }
        }

        private class ArgumentParserContext : OptionParserContext
        {
            public ArgumentParserContext(Queue<ArgumentToken> tokens, Command<TResult> command, object args, Option<TResult> option, List<string> values)
                : base(tokens, command, args, option)
            {
                Values = values;
            }

            public List<string> Values { get; }
        }
    }
}