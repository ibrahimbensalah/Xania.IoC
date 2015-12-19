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

        [MethodImpl(MethodImplOptions.NoInlining)]
        public ConventionBasedResolver()
        {
            _assemblies = new[] { Assembly.GetCallingAssembly() };
        }

        public ConventionBasedResolver(params Assembly[] assemblies)
        {
            _assemblies = new List<Assembly>(assemblies);
        }

        public void RegisterAssembly(Assembly assembly)
        {
            if (assembly == null) 
                throw new ArgumentNullException("assembly");

            _assemblies.Add(assembly);
        }

        public IResolvable Resolve(Type type)
        {
            var implementationType  = GetImplementationType(type);
            if (implementationType == null)
                return null;

            return TypeResolvable.Create(implementationType);
        }

        private Type GetImplementationType(Type interfaceType)
        {
            var stack = new Stack<Type>();
            stack.Push(interfaceType);

            var allTypes = _assemblies.SelectMany(a => a.GetExportedTypes()).ToArray();

            while (stack.Count > 0)
            {
                var sourceType = stack.Pop();

                if (sourceType.IsInterface || sourceType.IsAbstract || sourceType.GetConstructors().Length == 0)
                {
                    foreach (var subtype in allTypes.Where(t => t.GetInterfaces().Contains(sourceType) || t.BaseType == sourceType))
                        stack.Push(subtype);
                }
                else
                {
                    return sourceType;
                }
            }

            return null;
        }
    }
}