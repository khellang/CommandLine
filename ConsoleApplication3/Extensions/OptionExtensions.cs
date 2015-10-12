using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using ConsoleApplication3.Parsing;

namespace ConsoleApplication3.Extensions
{
    internal static class OptionExtensions
    {
        private static readonly ConcurrentDictionary<Type, Type> GenericListTypes = new ConcurrentDictionary<Type, Type>();

        private static readonly Func<Type, Type> GenericListTypeFactory = type => typeof(List<>).MakeGenericType(type);

        public static bool IsList<TResult>(this MappedProperty<TResult> property)
        {
            return property.Property.PropertyType.IsEnumerable()
                && property.Property.PropertyType != typeof(string);
        }

        public static bool IsFlag<TResult>(this MappedProperty<TResult> property)
        {
            return property.Property.PropertyType == typeof(bool);
        }

        public static void SetValue<TResult>(this MappedProperty<TResult> property, object target, string value)
        {
            property.Property.SetValue(target, property.ConvertValue(value, property.Property.PropertyType));
        }

        public static void SetValues<TResult>(this MappedProperty<TResult> property, object target, string[] values)
        {
            var itemType = property.Property.PropertyType.GetGenericTypeArgument();

            var genericType = GenericListTypes.GetOrAdd(itemType, GenericListTypeFactory);

            var list = (IList) Activator.CreateInstance(genericType);

            foreach (var value in values)
            {
                list.Add(property.ConvertValue(value, itemType));
            }

            property.Property.SetValue(target, list);
        }

        private static object ConvertValue<TResult>(this MappedProperty<TResult> property, string value, Type type)
        {
            var converter = TypeDescriptor.GetConverter(type);

            try
            {
                return converter.ConvertFromString(null, property.Config.CultureInfo, value);
            }
            catch (Exception ex)
            {
                throw new ArgumentParserException<TResult>($"Failed to parse option '{property.Name}': {ex.Message}");
            }
        }
    }
}