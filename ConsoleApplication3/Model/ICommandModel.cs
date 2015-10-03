using System;
using System.Collections.Generic;

namespace ConsoleApplication3.Model
{
    internal interface ICommandModel
    {
        Type Type { get; }

        string Name { get; }
    }

    internal interface ICommandModel<TResult> : ICommandModel
    {
        Func<object, TResult> Execute { get; }

        IReadOnlyDictionary<string, IOptionModel<TResult>> Options { get; }
    }
}