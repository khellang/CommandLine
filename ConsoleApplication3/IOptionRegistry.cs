﻿using System;
using System.Linq.Expressions;

namespace ConsoleApplication3
{
    public interface IOptionRegistry<TArgs, out TRegistry> where TRegistry : IOptionRegistry<TArgs, TRegistry>
    {
        TRegistry AddOption<TProperty>(string names, Expression<Func<TArgs, TProperty>> mapping);
    }

    public interface IOptionRegistry<TArgs, TResult, out TRegistry> : IOptionRegistry<TArgs, TRegistry>
        where TRegistry : IOptionRegistry<TArgs, TResult, TRegistry> { }
}