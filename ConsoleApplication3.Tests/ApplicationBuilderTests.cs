using System;
using Xunit;

namespace ConsoleApplication3.Tests
{
    public class ApplicationBuilderTests
    {
        private static readonly Func<object, int> NoOp = _ => 0;

        private readonly ApplicationBuilder<int> _builder = new ApplicationBuilder<int>(new Configuration<int>());

        [Theory]
        [InlineData("hello", "hello")]
        [InlineData("  hello  ", "hello")]
        [InlineData("some-command  ", "some-command")]
        [InlineData("   123-command", "123-command")]
        public void ShouldParseName(string actual, string result)
        {
            _builder.AddCommand<object>(actual, _ => NoOp);

            Command<int> command;
            Assert.Equal(1, _builder.CommandRegistry.Commands.Count);
            Assert.True(_builder.CommandRegistry.Commands.TryGetValue(result, out command));
        }

        [Fact]
        public void ShouldThrowForInvalidCharacters()
        {
            var exception = Assert.Throws<FormatException>(() => _builder.AddCommand<object>("asd#!%&/\".,", _ => NoOp));

            Assert.Contains("The command name", exception.Message);
        }
    }
}