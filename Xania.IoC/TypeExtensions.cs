using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Xania.IoC
{
    internal static class TypeExtensions
    {
        public static IEnumerable<Type> GetInterfaces(this Type type, bool includeInherited)
        {
            if (includeInherited || type.BaseType == null)
                return type.GetInterfaces();
            if (type.BaseType != null)
                return type.GetInterfaces().Except(type.BaseType.GetInterfaces());
            return Enumerable.Empty<Type>();
        }

        public static bool IsConcrete(this Type type)
        {
            return (type.IsInterface || type.IsAbstract || type.GetConstructors().Length == 0);
        }


        public static Type MapTo(this Type sourceType, Type targetType)
        {
            if (sourceType.IsGenericTypeDefinition)
                throw new ArgumentException("sourceType cannot be a generic type definition");

            if (sourceType.IsAssignableFrom(targetType))
                return targetType;

            if (!targetType.IsGenericTypeDefinition)
                return null;

            if (sourceType.IsGenericType)
            {
                if (targetType.GetGenericTypeDefinition() == sourceType.GetGenericTypeDefinition())
                {
                    return targetType.MakeGenericType(sourceType.GenericTypeArguments);
                }

                foreach (var i in targetType.GetInterfaces(false))
                {
                    if (i.IsGenericType && (i.GetGenericTypeDefinition() == sourceType.GetGenericTypeDefinition()))
                    {
                        
                    }

                }
                    
            }

            return null;
        }

    }
}
