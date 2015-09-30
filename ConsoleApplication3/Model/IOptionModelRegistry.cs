using System;
using System.Linq.Expressions;

namespace ConsoleApplication3
{
    public interface IOptionModelRegistry<TArgs, out TRegistry> where TRegistry : IOptionModelRegistry<TArgs, TRegistry>
    {
        TRegistry MapOption<TProperty>(string option, Expression<Func<TArgs, TProperty>> mapping);
    }

    public interface IOptionModelRegistry<TArgs, TResult, out TRegistry> : IOptionModelRegistry<TArgs, TRegistry>
        where TRegistry : IOptionModelRegistry<TArgs, TResult, TRegistry> { }
}