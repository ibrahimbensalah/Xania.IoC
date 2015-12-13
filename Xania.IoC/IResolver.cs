using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Xania.IoC
{
    public interface IResolver
    {
        IResolvable Resolve(Type type);
    }

    public class ConventionBasedResolver: IResolver
    {
        private readonly List<Assembly> _assemblies;

        public ConventionBasedResolver()
        {
            _assemblies = new List<Assembly>();
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

            return ConstructorResolvable.Create(implementationType);
        }

        private Type GetImplementationType(Type sourceType)
        {
            var stack = new Stack<Type>();
            stack.Push(sourceType);

            var allTypes = sourceType.Assembly.GetExportedTypes();

            while (stack.Count > 0)
            {
                var type = stack.Pop();

                if (type.IsInterface || type.IsAbstract || type.GetConstructors().Length == 0)
                {
                    foreach (var subtype in allTypes.Where(e => e.BaseType == type))
                        stack.Push(subtype);
                }
                else
                {
                    return type;
                }
            }

            return null;
        }
    }
}