using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ConsoleApplication3.Extensions;

namespace ConsoleApplication3
{
    internal class ArgumentRegistry<TArgs, TResult, TBuilder> : IArgumentRegistry<TArgs, TBuilder>
        where TBuilder : IArgumentRegistry<TArgs, TBuilder>, IConfigurable<TResult>
    {
        public ArgumentRegistry(TBuilder builder)
        {
            Builder = builder;
            Arguments = new List<MappedProperty<TResult>>();
        }

        public TBuilder Builder { get; }

        public List<MappedProperty<TResult>> Arguments { get; }

        public TBuilder AddArgument<TProperty>(string name, Expression<Func<TArgs, TProperty>> mapping)
        {
            var property = mapping.GetWritableProperty();

            var trimmed = name.Trim();

            if (!trimmed.IsValidName())
            {
                throw new FormatException($"The argument name '{trimmed}' is invalid.");
            }

            Arguments.Add(new MappedProperty<TResult>(Builder.Config, trimmed, property));

            return Builder;
        }
    }
}