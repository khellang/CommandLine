using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ConsoleApplication3.Extensions;

namespace ConsoleApplication3
{
    internal class OptionRegistry<TArgs, TResult, TBuilder> : IOptionRegistry<TArgs, TBuilder>
        where TBuilder : IOptionRegistry<TArgs, TBuilder>, IConfigurable<TResult>
    {
        public OptionRegistry(TBuilder builder)
        {
            Builder = builder;
            Options = new Dictionary<string, MappedProperty<TResult>>(builder.Config.StringComparer);
        }

        public TBuilder Builder { get; }

        public Dictionary<string, MappedProperty<TResult>> Options { get; }

        public TBuilder Option<TProperty>(string names, Expression<Func<TArgs, TProperty>> mapping)
        {
            var property = mapping.GetWritableProperty();

            foreach (var name in names.Split(Constants.OptionNameSeparator))
            {
                var trimmed = name.Trim();

                if (!trimmed.IsValidName())
                {
                    throw new FormatException($"The option name '{trimmed}' is invalid.");
                }

                Options.Add(trimmed, new MappedProperty<TResult>(Builder.Config, trimmed, property));
            }

            return Builder;
        }
    }
}