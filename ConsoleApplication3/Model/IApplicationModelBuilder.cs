using System;

namespace ConsoleApplication3.Model
{
    public interface IApplicationModelBuilder<TResult>
    {
        IApplicationModelBuilder<TResult> AddCommand<TArgs>(string name,
            Func<ICommandModelBuilder<TArgs, TResult>, Func<TArgs, TResult>> build)
            where TArgs : new();
    }
}