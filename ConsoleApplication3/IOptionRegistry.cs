﻿using System;
using System.Linq.Expressions;

namespace ConsoleApplication3
{
    public interface IOptionRegistry<TArgs, out TBuilder> where TBuilder : IOptionRegistry<TArgs, TBuilder>
    {
        TBuilder Option<TProperty>(string names, Expression<Func<TArgs, TProperty>> mapping);
    }
}