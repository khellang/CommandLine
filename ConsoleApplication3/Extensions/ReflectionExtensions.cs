using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApplication3.Extensions
{
    internal static class ReflectionExtensions
    {
        public static bool IsEnumerable(this Type type)
        {
            return type != typeof(string) && type.DerivesFromGenericTypeDefinition(typeof(IEnumerable<>));
        }

        public static Type GetGenericTypeArgument(this Type type)
        {
            return type.IsGenericType ? type.GetGenericArguments().Single() : null;
        }

        private static bool DerivesFromGenericTypeDefinition(this Type type, Type genericTypeDefinition)
        {
            var interfaceTypes = type.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.HasGenericTypeDefinition(genericTypeDefinition))
                {
                    return true;
                }
            }

            if (type.HasGenericTypeDefinition(genericTypeDefinition))
            {
                return true;
            }

            var baseType = type.BaseType;

            if (baseType == null)
            {
                return false;
            }

            return baseType.DerivesFromGenericTypeDefinition(genericTypeDefinition);
        }

        private static bool HasGenericTypeDefinition(this Type type, Type genericTypeDefinition)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == genericTypeDefinition;
        }
    }
}