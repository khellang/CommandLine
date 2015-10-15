using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static ConsoleApplication3.Parsing.ArgumentToken;

namespace ConsoleApplication3.Parsing
{
    internal class ArgumentLexer<TResult>
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

            foreach (var arg in ExpandResponseFiles(args, Config.ResponseFileReader))
            {
                if (escapeOptions)
                {
                    tokens.Add(Literal(arg, Config.StringComparer));
                    continue;
                }

                if (arg == "--")
                {
                    escapeOptions = true;
                    continue;
                }

                foreach (var token in ParseTokens(Config, arg))
                {
                    ArgumentToken[] exploded;
                    if (TryExpandSwitchGroup(token, Config.StringComparer, out exploded))
                    {
                        tokens.AddRange(exploded);
                        continue;
                    }

                    tokens.Add(token);
                }
            }

            return tokens.ToArray();
        }

        private static IEnumerable<string> ExpandResponseFiles(IEnumerable<string> args, Func<string, IEnumerable<string>> reader, string directory = null)
        {
            foreach (var arg in args)
            {
                if (arg.StartsWith("@"))
                {
                    var path = arg.Substring(1);

                    var fullPath = Path.Combine(directory ?? string.Empty, path);

                    if (string.IsNullOrEmpty(directory))
                    {
                        directory = Path.GetDirectoryName(path);
                    }

                    foreach (var expandedArg in ExpandResponseFiles(reader(fullPath), reader, directory))
                    {
                        yield return Environment.ExpandEnvironmentVariables(expandedArg).Trim();
                    }
                }
                else
                {
                    yield return arg;
                }
            }
        }

        private static bool TryExpandSwitchGroup(ArgumentToken token, StringComparer comparer, out ArgumentToken[] tokens)
        {
            if (token.IsSwitch && token.Value.Length > 1)
            {
                tokens = token.Value.Select(option => Option("-", option.ToString(), comparer)).ToArray();
                return true;
            }

            tokens = null;
            return false;
        }

        private static ArgumentToken[] ParseTokens(ApplicationConfiguration<TResult> config, string arg)
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
                    tokens.Add(Option(modifier, name, config.StringComparer));

                    if (!string.IsNullOrEmpty(value))
                    {
                        tokens.Add(Literal(value, config.StringComparer));
                    }
                }
                else
                {
                    tokens.Add(Option(modifier, nameValue, config.StringComparer));
                }
            }
            else
            {
                tokens.Add(Literal(arg, config.StringComparer));
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