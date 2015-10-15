using System;

namespace ConsoleApplication3
{
    public interface ICommandRegistry<TResult, out TBuilder> where TBuilder : ICommandRegistry<TResult, TBuilder>
    {
        TBuilder AddCommand<TArgs>(string name, Func<ICommandBuilder<TArgs, TResult>, Func<TArgs, TResult>> build);
    }
}