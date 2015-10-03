using System.Collections.Generic;

namespace ConsoleApplication3.Model
{
    internal interface IApplicationModel<TResult>
    {
        IReadOnlyDictionary<string, ICommandModel<TResult>> Commands { get; }

        TResult Run(string[] args);
    }
}