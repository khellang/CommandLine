using System;
using System.Collections.Generic;
using ConsoleApplication3.Parsing;
using Xunit;

namespace ConsoleApplication3.Tests
{
    public class ArgumentParserTests
    {
        public ArgumentParserTests()
        {
            var config = new ApplicationConfiguration<int>();

            var commands = GetCommands(app =>
            {
                app.AddCommand<Args>("command", cmd =>
                {
                    cmd.AddOption("b|boolean", x => x.Boolean);
                    cmd.AddOption("f|flag", x => x.Flag);
                    cmd.AddOption("s|string", x => x.String);
                    cmd.AddOption("i|integer", x => x.Integer);
                    cmd.AddOption("d|double", x => x.Double);
                    cmd.AddOption("string-list", x => x.StringList);
                    cmd.AddOption("integer-list", x => x.IntegerList);

                    return args => 0;
                });
            });

            Lexer = new ArgumentLexer<int>(config);
            Parser = new ArgumentParser<int>(config, commands);
        }

        private ArgumentLexer<int> Lexer { get; }

        private ArgumentParser<int> Parser { get; }

        [Fact]
        public void ShouldParseStringOption()
        {
            var result = Parse<Args>("command -s hello");

            Assert.Equal("hello", result.String);
        }

        [Fact]
        public void ShouldParseFlagOption()
        {
            var result = Parse<Args>("command --boolean");

            Assert.True(result.Boolean);
        }

        [Fact]
        public void ShouldParseFlagWithExplicitValue()
        {
            var result = Parse<Args>("command -b:true");

            Assert.True(result.Boolean);
        }

        [Fact]
        public void ShouldParseGroupedFlags()
        {
            var result = Parse<Args>("command -bf");

            Assert.True(result.Flag);
            Assert.True(result.Boolean);
        }

        [Fact]
        public void ShouldParseLastGroupedFlagValue()
        {
            var result = Parse<Args>("command -bs hello");

            Assert.True(result.Boolean);
            Assert.Equal("hello", result.String);
        }

        [Fact]
        public void ShouldParseListValues()
        {
            var result = Parse<Args>("command --integer-list 1 2 3 4 --string-list hello world");

            Assert.Equal(new [] { 1, 2, 3, 4 }, result.IntegerList);
            Assert.Equal(new [] { "hello", "world" }, result.StringList);
        }

        [Fact]
        public void ShouldThrowForInvalidArgumentType()
        {
            var exception = Assert.Throws<ArgumentParserException<int>>(() => Parse<Args>("command -i hello"));

            Assert.Contains("Failed to parse option", exception.Message);
        }

        [Fact]
        public void ShouldThrowForPositionalArguments()
        {
            var exception = Assert.Throws<ArgumentParserException<int>>(() => Parse<Args>("command -s hello world"));

            Assert.Contains("Invalid argument", exception.Message);
        }

        [Fact]
        public void ShouldThrowForUnknownCommand()
        {
            var exception = Assert.Throws<ArgumentParserException<int>>(() => Parse<Args>("hello"));

            Assert.Contains("Unknown command", exception.Message);
        }

        [Theory]
        [InlineData("command -s")]
        [InlineData("command -i -s hello")]
        [InlineData("command --string:")]
        public void ShouldThrowForMissingValue(string arguments)
        {
            var exception = Assert.Throws<ArgumentParserException<int>>(() => Parse<Args>(arguments));

            Assert.Contains("requires a value", exception.Message);
        }

        [Fact]
        public void ShouldThrowForUnknownOption()
        {
            var exception = Assert.Throws<ArgumentParserException<int>>(() => Parse<Args>("command --asdf"));

            Assert.Contains("Unknown option", exception.Message);
        }

        [Fact]
        public void ShouldThrowForEmptyArguments()
        {
            var exception = Assert.Throws<ArgumentParserException<int>>(() => Parse<Args>(string.Empty));

            Assert.Contains("Please specify a command", exception.Message);
        }

        private static IReadOnlyDictionary<string, Command<int>> GetCommands(Action<IApplicationBuilder<int>> build)
        {
            var config = new ApplicationConfiguration<int>
            {
                HandleErrors = false
            };

            return Application.Create(config, build).Commands;
        }

        private T Parse<T>(string args)
        {
            return (T) Parser.Parse(Lexer.Lex(args
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))).Args;
        }

        private class Args
        {
            public bool Boolean { get; set; }

            public bool Flag { get; set; }

            public string String { get; set; }

            public int Integer { get; set; }

            public double Double { get; set; }

            public IEnumerable<string> StringList { get; set; }

            public IEnumerable<int> IntegerList { get; set; }
        }
    }
}