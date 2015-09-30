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
            var commands = GetCommands(app =>
            {
                app.AddCommand<Args>("command", cmd =>
                {
                    cmd.MapOption("b|boolean", x => x.Boolean);
                    cmd.MapOption("f|flag", x => x.Flag);
                    cmd.MapOption("s|string", x => x.String);
                    cmd.MapOption("i|integer", x => x.Integer);
                    cmd.MapOption("d|double", x => x.Double);
                    cmd.MapOption("string-list", x => x.StringList);
                    cmd.MapOption("int-list", x => x.IntegerList);

                    return args => 0;
                });
            });

            Parser = new ArgumentParser<int>(commands);
        }

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
            var result = Parse<Args>("command --int-list 1 2 3 4 --string-list hello world");

            Assert.Equal(new [] { 1, 2, 3, 4 }, result.IntegerList);
            Assert.Equal(new [] { "hello", "world" }, result.StringList);
        }

        [Fact]
        public void ShouldThrowForInvalidArgumentType()
        {
            Assert.Throws<ArgumentParserException>(() => Parse<Args>("command -i hello"));
        }

        [Fact]
        public void ShouldThrowForPositionalArguments()
        {
            Assert.Throws<ArgumentParserException>(() => Parse<Args>("command -s hello world"));
        }

        [Fact]
        public void ShouldThrowForUnknownCommand()
        {
            Assert.Throws<ArgumentParserException>(() => Parse<Args>("hello"));
        }

        [Theory]
        [InlineData("command -s")]
        [InlineData("command -i -s hello")]
        public void ShouldThrowForMissingValue(string arguments)
        {
            Assert.Throws<ArgumentParserException>(() => Parse<Args>(arguments));
        }

        [Fact]
        public void ShouldThrowForUnknownOption()
        {
            Assert.Throws<ArgumentParserException>(() => Parse<Args>("command --asdf"));
        }

        [Fact]
        public void ShouldThrowForEmptyArguments()
        {
            Assert.Throws<ArgumentParserException>(() => Parse<Args>(string.Empty));
        }

        private static IReadOnlyDictionary<string, ICommandModel<int>> GetCommands(Action<IApplicationModelBuilder<int>> build)
        {
            var config = new ApplicationConfiguration
            {
                HandleErrors = false
            };

            return Application.Create(config, build).Commands;
        }

        private T Parse<T>(string args)
        {
            return (T) Parser.Parse(ArgumentLexer.Lex(
                args.Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries))).Args;
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