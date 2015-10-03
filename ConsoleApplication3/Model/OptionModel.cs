using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using ConsoleApplication3.Extensions;

namespace ConsoleApplication3.Model
{
    [DebuggerDisplay("{Name, nq}")]
    internal sealed class OptionModel : IOptionModel
    {
        private static readonly char[] NameSeparator = { '|' };

        private OptionModel(string name, PropertyInfo property)
        {
            Name = name;
            Property = property;
        }

        public string Name { get; }

        public PropertyInfo Property { get; }

        public static IDictionary<string, OptionModel> Create<TTarget, TProperty>(string option, Expression<Func<TTarget, TProperty>> mapping)
        {
            var property = mapping.GetProperty();

            if (!property.CanWrite)
            {
                throw new ArgumentException($"Mapped property '{property.Name}' is read-only.", nameof(mapping));
            }

            var names = option.Split(NameSeparator);

            var options = new Dictionary<string, OptionModel>(StringComparer.Ordinal);

            foreach (var name in names)
            {
                var trimmedName = name.Trim();

                if (!Utilities.IsValidName(trimmedName))
                {
                    throw new ArgumentException($"The option name '{trimmedName}' is invalid.", nameof(option));
                }

                options.Add(trimmedName, new OptionModel(trimmedName, property));
            }

            return options;
        }
    }
}