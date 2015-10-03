using ConsoleApplication3.Model;

namespace ConsoleApplication3.Parsing
{
    internal sealed class ArgumentParserResult<TResult>
    {
        public ArgumentParserResult(object args, ICommandModel<TResult> command)
        {
            Args = args;
            Command = command;
        }

        public object Args { get; }

        public ICommandModel<TResult> Command { get; }
    }
}