using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using ConsoleApplication3.Model;
using ConsoleApplication3.Parsing;

namespace ConsoleApplication3.Extensions
{
    internal static class OptionModelExtensions
    {
        public static bool IsList(this IOptionModel optionModel)
        {
            return optionModel.Property.PropertyType.IsEnumerable()
                && optionModel.Property.PropertyType != typeof(string);
        }

        public static bool IsFlag(this IOptionModel optionModel)
        {
            return optionModel.Property.PropertyType == typeof(bool);
        }

        public static void SetValue(this IOptionModel optionModel, object target, string value)
        {
            var convertedValue = ConvertValue(optionModel, value, optionModel.Property.PropertyType);

            optionModel.Property.SetValue(target, convertedValue);
        }

        public static void SetValues(this IOptionModel optionModel, object target, string[] values)
        {
            var itemType = optionModel.Property.PropertyType.GetGenericTypeArgument();

            var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType));

            foreach (var value in values)
            {
                list.Add(ConvertValue(optionModel, value, itemType));
            }

            optionModel.Property.SetValue(target, list);
        }

        private static object ConvertValue(IOptionModel optionModel, string value, Type type)
        {
            var converter = TypeDescriptor.GetConverter(type);

            try
            {
                return converter.ConvertFromInvariantString(value);
            }
            catch (Exception ex)
            {
                throw new ArgumentParserException($"Failed to parse option '{optionModel.Name}': {ex.Message}");
            }
        }
    }
}