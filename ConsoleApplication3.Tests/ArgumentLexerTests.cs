using System;
using System.IO;
using ConsoleApplication3.Parsing;
using Xunit;

namespace ConsoleApplication3.Tests
{
    public class ArgumentLexerTests
    {
        private static Configuration<int> Config { get; } = new Configuration<int>();

        private ArgumentLexer<int> Lexer { get; } = new ArgumentLexer<int>(Config);

        [Fact]
        public void ShouldLexLiterals()
        {
            Lex("hello world", Literal("hello"), Literal("world"));
        }

        [Fact]
        public void ShouldLexOptions()
        {
            Lex("-a --test", Option("-", "a"), Option("--", "test"));
        }

        [Fact]
        public void ShouldLexResponseFiles()
        {
            Lex(Path.Combine("@ResponseFiles", "test.rsp"),
                Option("-", "s"),
                Literal("hello"),
                Literal("1"),
                Literal("2"),
                Literal("3"),
                Literal("4"),
                Literal("5"),
                Literal("6"));
        }

        [Fact]
        public void ShouldLexOptionsWithArguments()
        {
            Lex("-a:true -b=hello --test ok",
                Option("-", "a"),
                Literal("true"),
                Option("-", "b"),
                Literal("hello"),
                Option("--", "test"),
                Literal("ok"));
        }

        [Fact]
        public void ShouldExplodeBundles()
        {
            Lex("-abc", Option("-", "a"), Option("-", "b"), Option("-", "c"));
        }

        [Fact]
        public void ShouldNotExpandDoubleDashOptions()
        {
            Lex("--abc", Option("--", "abc"));
        }

        [Fact]
        public void ShouldEscapeOptionsWithDoubleDashSeparator()
        {
            Lex("--hello -- -abc", Option("--", "hello"), Literal("-abc"));
        }

        [Fact]
        public void ShouldReturnEmptyArrayForEmptyArguments()
        {
            Lex(string.Empty);
        }

        [Fact]
        public void ShouldCorrectlyHandleMissingValues()
        {
            Lex("--string:", Option("--", "string"));
        }

        private void Lex(string args, params ArgumentToken[] expected)
        {
            Assert.Equal(expected, Lexer.Lex(args.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)));
        }

        private static ArgumentToken Literal(string value)
        {
            return ArgumentToken.Literal(value, Config.StringComparer);
        }

        private static ArgumentToken Option(string modifier, string name)
        {
            return ArgumentToken.Option(modifier, name, Config.StringComparer);
        }
    }
}