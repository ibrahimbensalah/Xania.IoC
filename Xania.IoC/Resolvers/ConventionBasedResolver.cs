using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Xania.IoC.Resolvers
{
    public class ConventionBasedResolver: IResolver
    {
        private readonly ICollection<Assembly> _assemblies;
        private Type[] _allTypes;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public ConventionBasedResolver()
        {
            _assemblies = new[] { Assembly.GetCallingAssembly() };
        }

        public ConventionBasedResolver(params Assembly[] assemblies)
        {
            _assemblies = new List<Assembly>(assemblies);
        }

        public IResolvable Resolve(Type type)
        {
            var implementationType  = GetImplementationType(type);
            if (implementationType == null)
                return null;

            return TypeResolvable.Create(implementationType);
        }

        private Type[] AllTypes
        {
            get
            {
                if (_allTypes == null)
                    _allTypes = _assemblies.SelectMany(a => a.GetExportedTypes()).ToArray();
                return _allTypes;
            }
        }

        private Type GetImplementationType(Type interfaceType)
        {
            var stack = new Stack<Type>();
            stack.Push(interfaceType);

            while (stack.Count > 0)
            {
                var sourceType = stack.Pop();

                if (sourceType.IsInterface || sourceType.IsAbstract || sourceType.GetConstructors().Length == 0)
                {
                    foreach (var subtype in AllTypes.Where(t => GetInterfaces(t, false).Contains(sourceType) || t.BaseType == sourceType))
                        stack.Push(subtype);
                }
                else
                {
                    return sourceType;
                }
            }

            return null;
        }

        public static IEnumerable<Type> GetInterfaces(Type type, bool includeInherited)
        {
            if (includeInherited || type.BaseType == null)
                return type.GetInterfaces();
            else
                return type.GetInterfaces().Except(type.BaseType.GetInterfaces());
        }
    }
}