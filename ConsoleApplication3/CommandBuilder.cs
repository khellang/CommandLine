using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ConsoleApplication3.Extensions;

namespace ConsoleApplication3
{
    internal sealed class CommandBuilder<TArgs, TResult> : ICommandBuilder<TArgs, TResult>
    {
        public CommandBuilder(ApplicationConfiguration<TResult> config, string name)
        {
            Config = config;
            Name = name;
            Options = new Dictionary<string, MappedProperty<TResult>>(config.StringComparer);
            Arguments = new List<MappedProperty<TResult>>();
        }

        private ApplicationConfiguration<TResult> Config { get; }

        private string Name { get; }

        private List<MappedProperty<TResult>> Arguments { get; }

        private Dictionary<string, MappedProperty<TResult>> Options { get; }

        public ICommandBuilder<TArgs, TResult> AddOption<TProperty>(string names, Expression<Func<TArgs, TProperty>> mapping)
        {
            var property = GetWritableProperty(mapping);

            foreach (var name in names.Split(Constants.OptionNameSeparator))
            {
                var trimmed = name.Trim();

                if (!trimmed.IsValidName())
                {
                    throw new FormatException($"The option name '{trimmed}' is invalid.");
                }

                Options.Add(trimmed, new MappedProperty<TResult>(Config, trimmed, property));
            }

            return this;
        }

        public ICommandBuilder<TArgs, TResult> AddArgument<TProperty>(string name, Expression<Func<TArgs, TProperty>> mapping)
        {
            var property = GetWritableProperty(mapping);

            var trimmed = name.Trim();

            if (!trimmed.IsValidName())
            {
                throw new FormatException($"The argument name '{trimmed}' is invalid.");
            }

            Arguments.Add(new MappedProperty<TResult>(Config, trimmed, property));

            return this;
        }

        public Command<TResult> Build(Func<TArgs, TResult> execute)
        {
            return new Command<TResult>(typeof(TArgs), Name, args => execute((TArgs) args), Options, Arguments);
        }

        private static PropertyInfo GetWritableProperty<TProperty>(Expression<Func<TArgs, TProperty>> mapping)
        {
            var property = mapping.GetProperty();

            if (!property.CanWrite)
            {
                throw new ArgumentException($"Mapped property '{property.Name}' is read-only.", nameof(mapping));
            }

            return property;
        }
    }
}