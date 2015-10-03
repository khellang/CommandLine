using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ConsoleApplication3.Model
{
    [DebuggerDisplay("{Name, nq}")]
    internal sealed class CommandModel<TResult> : ICommandModel<TResult>
    {
        private CommandModel(Type type,
            string name,
            Func<object, TResult> execute,
            IReadOnlyDictionary<string, IOptionModel> options)
        {
            Type = type;
            Name = name;
            Options = options;
            Execute = execute;
        }

        public Type Type { get; }

        public string Name { get; }

        public Func<object, TResult> Execute { get; }

        public IReadOnlyDictionary<string, IOptionModel> Options { get; }

        public static CommandModel<TResult> Create<TArgs>(string name,
            IReadOnlyDictionary<string, IOptionModel> options,
            Func<TArgs, TResult> execute)
        {
            var trimmedName = name.Trim();

            if (!Utilities.IsValidName(trimmedName))
            {
                throw new ArgumentException($"The command name '{trimmedName}' is invalid.", nameof(trimmedName));
            }

            return new CommandModel<TResult>(typeof(TArgs), trimmedName, args => execute((TArgs) args), options);
        }
    }
}