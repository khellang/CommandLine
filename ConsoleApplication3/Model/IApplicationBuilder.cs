using System;

namespace ConsoleApplication3.Model
{
    public interface IApplicationBuilder<TResult>
    {
        IApplicationBuilder<TResult> AddCommand<TArgs>(string name, Func<ICommandBuilder<TArgs, TResult>, Func<TArgs, TResult>> build);
    }
}