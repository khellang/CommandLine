namespace ConsoleApplication3.Model
{
    public interface ICommandBuilder<TArgs, TResult> : IOptionRegistry<TArgs, TResult, ICommandBuilder<TArgs, TResult>> { }
}