namespace ConsoleApplication3
{
    public interface ICommandBuilder<TArgs, TResult> :
        IOptionRegistry<TArgs, ICommandBuilder<TArgs, TResult>>,
        IArgumentRegistry<TArgs, ICommandBuilder<TArgs, TResult>>,
        IConfigurable<TResult>
    {
    }
}