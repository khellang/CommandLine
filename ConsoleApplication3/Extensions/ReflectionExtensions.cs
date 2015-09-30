using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApplication3
{
    internal static class ReflectionExtensions
    {
        public static bool IsEnumerable(this Type type)
        {
            return type.HasGenericTypeDefinition(typeof(IEnumerable<>)) && type != typeof(string);
        }

        public static Type GetGenericTypeArgument(this Type type)
        {
            return type.IsGenericType ? type.GetGenericArguments().Single() : null;
        }

        private static bool HasGenericTypeDefinition(this Type type, Type genericTypeDefinition)
        {
            var interfaceTypes = type.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericTypeDefinition)
                {
                    return true;
                }
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == genericTypeDefinition)
            {
                return true;
            }

            var baseType = type.BaseType;

            if (baseType == null)
            {
                return false;
            }

            return HasGenericTypeDefinition(baseType, genericTypeDefinition);
        }
    }
}