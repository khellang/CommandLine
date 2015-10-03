namespace ConsoleApplication3.Model
{
    public interface ICommandModelBuilder<TArgs, TResult>
        : IOptionModelRegistry<TArgs, TResult, ICommandModelBuilder<TArgs, TResult>>
        where TArgs : new() { }
}