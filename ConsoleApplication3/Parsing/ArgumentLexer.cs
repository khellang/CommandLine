using System.Collections.Generic;

namespace ConsoleApplication3.Parsing
{
    internal sealed class ArgumentLexer<TResult>
    {
        public ArgumentLexer(ApplicationConfiguration<TResult> config)
        {
            Config = config;
        }

        private ApplicationConfiguration<TResult> Config { get; }

        public ArgumentToken[] Lex(string[] args)
        {
            var tokens = new List<ArgumentToken>();

            if (args.Length == 0)
            {
                return tokens.ToArray();
            }

            var escapeOptions = false;

            foreach (var arg in args)
            {
                if (escapeOptions)
                {
                    tokens.Add(ArgumentToken.Literal(arg));
                    continue;
                }

                if (arg == "--")
                {
                    escapeOptions = true;
                    continue;
                }

                foreach (var token in ParseTokens(arg))
                {
                    ArgumentToken[] exploded;
                    if (token.TryExplodeSwitchGroup(out exploded))
                    {
                        tokens.AddRange(exploded);
                        continue;
                    }

                    tokens.Add(token);
                }
            }

            return tokens.ToArray();
        }

        private static ArgumentToken[] ParseTokens(string arg)
        {
            var tokens = new List<ArgumentToken>(capacity: 2);

            string modifier;
            string nameValue;
            if (TryParseOption(arg, out modifier, out nameValue))
            {
                string name;
                string value;
                if (TrySplitNameValue(nameValue, out name, out value))
                {
                    tokens.Add(ArgumentToken.Option(modifier, name));

                    if (!string.IsNullOrEmpty(value))
                    {
                        tokens.Add(ArgumentToken.Literal(value));
                    }
                }
                else
                {
                    tokens.Add(ArgumentToken.Option(modifier, nameValue));
                }
            }
            else
            {
                tokens.Add(ArgumentToken.Literal(arg));
            }

            return tokens.ToArray();
        }

        private static bool TryParseOption(string arg, out string modifier, out string nameValue)
        {
            return TryParseOption(arg, "--", out modifier, out nameValue)
                || TryParseOption(arg, "-", out modifier, out nameValue);
        }

        private static bool TryParseOption(string arg, string prefix, out string modifier, out string nameValue)
        {
            if (arg.StartsWith(prefix))
            {
                nameValue = arg.Substring(prefix.Length);
                modifier = prefix;
                return true;
            }

            nameValue = null;
            modifier = null;
            return false;
        }

        private static bool TrySplitNameValue(string arg, out string name, out string value)
        {
            return TrySplitNameValue(arg, ':', out name, out value)
                || TrySplitNameValue(arg, '=', out name, out value);
        }

        private static bool TrySplitNameValue(string arg, char separator, out string name, out string value)
        {
            var index = arg.IndexOf(separator);

            if (index >= 0)
            {
                name = arg.Substring(0, index);
                value = arg.Substring(index + 1);
                return true;
            }

            name = null;
            value = null;
            return false;
        }
    }
}