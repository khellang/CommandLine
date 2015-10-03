using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using ConsoleApplication3.Extensions;

namespace ConsoleApplication3.Model
{
    [DebuggerDisplay("{Name, nq}")]
    internal sealed class OptionModel<TResult> : IOptionModel<TResult>
    {
        private static readonly char[] NameSeparator = { '|' };

        private OptionModel(ApplicationConfiguration<TResult> config, string name, PropertyInfo property)
        {
            Config = config;
            Name = name;
            Property = property;
        }

        public ApplicationConfiguration<TResult> Config { get; }

        public string Name { get; }

        public PropertyInfo Property { get; }

        public static IDictionary<string, IOptionModel<TResult>> Create<TTarget, TProperty>(
            ApplicationConfiguration<TResult> config,
            string option,
            Expression<Func<TTarget, TProperty>> mapping)
        {
            var property = mapping.GetProperty();

            if (!property.CanWrite)
            {
                throw new ArgumentException($"Mapped property '{property.Name}' is read-only.", nameof(mapping));
            }

            var names = option.Split(NameSeparator);

            var options = new Dictionary<string, IOptionModel<TResult>>(config.StringComparer);

            foreach (var name in names)
            {
                var trimmedName = name.Trim();

                if (!trimmedName.IsValidName())
                {
                    throw new ArgumentException($"The option name '{trimmedName}' is invalid.", nameof(option));
                }

                options.Add(trimmedName, new OptionModel<TResult>(config, trimmedName, property));
            }

            return options;
        }
    }
}