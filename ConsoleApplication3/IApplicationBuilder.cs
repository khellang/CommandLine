namespace ConsoleApplication3
{
    public interface IApplicationBuilder<TResult> :
        ICommandRegistry<TResult, IApplicationBuilder<TResult>>,
        IConfigurable<TResult>
    {
    }

    public interface IApplicationBuilder<TArgs, TResult> :
        IOptionRegistry<TArgs, IApplicationBuilder<TArgs, TResult>>,
        IArgumentRegistry<TArgs, IApplicationBuilder<TArgs, TResult>>,
        ICommandRegistry<TResult, IApplicationBuilder<TArgs, TResult>>,
        IConfigurable<TResult>
    {
    }
}