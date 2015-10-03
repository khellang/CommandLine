using System;
using ConsoleApplication3.Parsing;
using Xunit;
using static ConsoleApplication3.Parsing.ArgumentToken;

namespace ConsoleApplication3.Tests
{
    public class ArgumentLexerTests
    {
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

        private static void Lex(string args, params ArgumentToken[] expected)
        {
            Assert.Equal(expected, ArgumentLexer.Lex(args.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)));
        }
    }
}