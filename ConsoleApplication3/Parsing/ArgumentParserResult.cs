namespace ConsoleApplication3.Parsing
{
    internal sealed class ArgumentParserResult<TResult>
    {
        public ArgumentParserResult(object args, Command<TResult> command)
        {
            Args = args;
            Command = command;
        }

        public object Args { get; }

        public Command<TResult> Command { get; }
    }
}