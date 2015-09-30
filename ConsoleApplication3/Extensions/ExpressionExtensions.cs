using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ConsoleApplication3
{
    internal static class ExpressionExtensions
    {
        public static PropertyInfo GetProperty<TTarget, TProperty>(this Expression<Func<TTarget, TProperty>> expression)
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