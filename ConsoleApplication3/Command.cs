using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ConsoleApplication3
{
    [DebuggerDisplay("{Name, nq}")]
    internal sealed class Command<TResult>
    {
        public Command(Type type, string name, Func<object, TResult> execute, IReadOnlyDictionary<string, Option<TResult>> options, IReadOnlyList<Argument<TResult>> arguments)
        {
            Type = type;
            Name = name;
            Options = options;
            Arguments = arguments;
            Execute = execute;
        }

        public Type Type { get; }

        public string Name { get; }

        public Func<object, TResult> Execute { get; }

        public IReadOnlyDictionary<string, Option<TResult>> Options { get; }

        public IReadOnlyList<Argument<TResult>> Arguments { get; }
    }
}