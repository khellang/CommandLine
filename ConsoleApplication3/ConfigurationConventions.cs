using System;
using System.Reflection;
using ConsoleApplication3.Extensions;

namespace ConsoleApplication3
{
    public class ConfigurationConventions
    {
        public ConfigurationConventions()
        {
            OptionNames = GetOptionNames;
        }

        public Func<PropertyInfo, string> OptionNames { get; set; }

        private static string GetOptionNames(PropertyInfo property)
        {
            var name = property.Name.KebabCase();
            return $"{name[0]}|{name}";
        }
    }
}