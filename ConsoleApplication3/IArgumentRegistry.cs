using System;
using System.Linq.Expressions;

namespace ConsoleApplication3
{
    public interface IArgumentRegistry<TArgs, out TBuilder> where TBuilder : IArgumentRegistry<TArgs, TBuilder>
    {
        TBuilder Argument<TProperty>(string name, Expression<Func<TArgs, TProperty>> mapping);
    }
}