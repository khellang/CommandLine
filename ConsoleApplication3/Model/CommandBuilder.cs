using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ConsoleApplication3.Extensions;

namespace ConsoleApplication3.Model
{
    internal sealed class CommandBuilder<TArgs, TResult> : ICommandBuilder<TArgs, TResult>
    {
        public CommandBuilder(ApplicationConfiguration<TResult> config, string name)
        {
            Config = config;
            Name = name;
            Options = new Dictionary<string, Option<TResult>>(config.StringComparer);
        }

        private ApplicationConfiguration<TResult> Config { get; }

        private string Name { get; }

        private Dictionary<string, Option<TResult>> Options { get; }

        public ICommandBuilder<TArgs, TResult> MapOption<TProperty>(string names, Expression<Func<TArgs, TProperty>> mapping)
        {
            return MapOption(names, mapping.GetProperty());
        }

        public Command<TResult> Build(Func<TArgs, TResult> execute)
        {
            var trimmedName = Name.Trim();

            if (!trimmedName.IsValidName())
            {
                throw new ArgumentException($"The command name '{trimmedName}' is invalid.", nameof(trimmedName));
            }

            return new Command<TResult>(typeof(TArgs), trimmedName, args => execute((TArgs)args), Options);
        }

        private ICommandBuilder<TArgs, TResult> MapOption(string names, PropertyInfo property)
        {
            var options = CreateOptions(names, property);

            foreach (var option in options)
            {
                Options.Add(option.Key, option.Value);
            }

            return this;
        }

        private IReadOnlyDictionary<string, Option<TResult>> CreateOptions(string names, PropertyInfo property)
        {
            if (!property.CanWrite)
            {
                throw new ArgumentException($"Mapped property '{property.Name}' is read-only.", nameof(property));
            }

            var options = new Dictionary<string, Option<TResult>>(Config.StringComparer);

            var splitNames = names.Split(Constants.OptionNameSeparator);

            foreach (var name in splitNames)
            {
                var trimmedName = name.Trim();

                if (!trimmedName.IsValidName())
                {
                    throw new ArgumentException($"The option name '{trimmedName}' is invalid.");
                }

                options.Add(trimmedName, new Option<TResult>(Config, trimmedName, property));
            }

            return options;
        }
    }
}