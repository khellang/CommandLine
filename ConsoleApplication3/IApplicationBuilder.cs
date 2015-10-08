using System;

namespace ConsoleApplication3
{
    public interface IApplicationBuilder<TResult>
    {
        IApplicationBuilder<TResult> AddCommand<TArgs>(string name, Func<ICommandBuilder<TArgs, TResult>, Func<TArgs, TResult>> build);
    }
}