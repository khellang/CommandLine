using System;
using System.Collections.Generic;

namespace ConsoleApplication3.Model
{
    internal interface ICommandModel
    {
        Type Type { get; }

        string Name { get; }

        IReadOnlyDictionary<string, IOptionModel> Options { get; }
    }

    internal interface ICommandModel<out TResult> : ICommandModel
    {
        Func<object, TResult> Execute { get; }
    }
}