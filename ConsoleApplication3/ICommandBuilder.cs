using System;
using System.Linq.Expressions;

namespace ConsoleApplication3
{
    public interface ICommandBuilder<TArgs, TResult> : IOptionRegistry<TArgs, TResult, ICommandBuilder<TArgs, TResult>>
    {
        ICommandBuilder<TArgs, TResult> AddArgument<TProperty>(string name, Expression<Func<TArgs, TProperty>> mapping);
    }
}