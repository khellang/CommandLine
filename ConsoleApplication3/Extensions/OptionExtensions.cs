using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using ConsoleApplication3.Model;
using ConsoleApplication3.Parsing;

namespace ConsoleApplication3.Extensions
{
    internal static class OptionExtensions
    {
        private static readonly ConcurrentDictionary<Type, Type> GenericListTypes = new ConcurrentDictionary<Type, Type>();

        private static readonly Func<Type, Type> GenericListTypeFactory = type => typeof(List<>).MakeGenericType(type);

        public static bool IsList<TResult>(this Option<TResult> option)
        {
            return option.Property.PropertyType.IsEnumerable()
                && option.Property.PropertyType != typeof(string);
        }

        public static bool IsFlag<TResult>(this Option<TResult> option)
        {
            return option.Property.PropertyType == typeof(bool);
        }

        public static void SetValue<TResult>(this Option<TResult> option, object target, string value)
        {
            option.Property.SetValue(target, option.ConvertValue(value, option.Property.PropertyType));
        }

        public static void SetValues<TResult>(this Option<TResult> option, object target, string[] values)
        {
            var itemType = option.Property.PropertyType.GetGenericTypeArgument();

            var genericType = GenericListTypes.GetOrAdd(itemType, GenericListTypeFactory);

            var list = (IList) Activator.CreateInstance(genericType);

            foreach (var value in values)
            {
                list.Add(option.ConvertValue(value, itemType));
            }

            option.Property.SetValue(target, list);
        }

        private static object ConvertValue<TResult>(this Option<TResult> option, string value, Type type)
        {
            var converter = TypeDescriptor.GetConverter(type);

            try
            {
                return converter.ConvertFromString(null, option.Config.CultureInfo, value);
            }
            catch (Exception ex)
            {
                throw new ArgumentParserException<TResult>($"Failed to parse option '{option.Name}': {ex.Message}");
            }
        }
    }
}