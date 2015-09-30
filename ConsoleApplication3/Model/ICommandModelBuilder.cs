namespace ConsoleApplication3
{
    public interface ICommandModelBuilder<TArgs, TResult>
        : IOptionModelRegistry<TArgs, TResult, ICommandModelBuilder<TArgs, TResult>>
        where TArgs : new() { }
}