﻿using System;
using ConsoleApplication3.Parsing;
using Xunit;

namespace ConsoleApplication3.Tests
{
    public class ArgumentLexerTests
    {
        private static ApplicationConfiguration<int> Config { get; } = new ApplicationConfiguration<int>();

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
            return ArgumentToken.Literal(Config.StringComparer, value);
        }

        private static ArgumentToken Option(string modifier, string name)
        {
            return ArgumentToken.Option(Config.StringComparer, modifier, name);
        }
    }
}