using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ConsoleApplication3.Extensions
{
    internal static class ExpressionExtensions
    {
        public static PropertyInfo GetWritableProperty<TArgs, TProperty>(this Expression<Func<TArgs, TProperty>> mapping)
        {
            var property = mapping.GetProperty();

            if (!property.CanWrite)
            {
                throw new ArgumentException($"Mapped property '{property.Name}' is read-only.", nameof(mapping));
            }

            return property;
        }

        private static PropertyInfo GetProperty<TTarget, TProperty>(this Expression<Func<TTarget, TProperty>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new InvalidOperationException("Expression must be a MemberExpression.");
            }

            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new InvalidOperationException("Expression member must be a PropertyInfo.");
            }

            return propertyInfo;
        }
    }
}