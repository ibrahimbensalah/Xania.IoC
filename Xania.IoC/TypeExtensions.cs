using System;
using System.Collections.Generic;
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
            return !(type.IsInterface || type.IsAbstract || type.GetConstructors().Length == 0);
        }

    }
}
