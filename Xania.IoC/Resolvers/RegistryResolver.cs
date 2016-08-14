using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Xania.IoC.Resolvers
{
    public class RegistryResolver : IResolver
    {
        private readonly HashSet<IRegistry> _registrations;

        public RegistryResolver()
        {
            _registrations = new HashSet<IRegistry>();
        }

        public virtual IEnumerable<IResolvable> ResolveAll(Type serviceType)
        {
            return
                from resolvable in _registrations.Select(r => r.Resolve(serviceType))
                where resolvable != null
                select resolvable;
        }

        private sealed class TypeRegistry: IRegistry
        {
            public string Name { get; private set; }
            public ConstructorArgs Args { get; set; }

            public IResolvable Resolve(Type targetType)
            {
                var concreteType = TemplateType.MapTo(targetType);
                if (concreteType != null)
                    return TypeResolvable.Create(concreteType, Args);
                return null;
            }

            public TypeRegistry(Type templateType, ConstructorArgs args, string name)
            {
                if (templateType == null) 
                    throw new ArgumentNullException("templateType");

                Name = name;
                Args = args;
                TemplateType = templateType;
            }

            private Type TemplateType { get; set; }

            // override object.Equals
            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }

                return Equals(obj as TypeRegistry);
                
            }

            private bool Equals(TypeRegistry typeRegistry)
            {
                return typeRegistry.TemplateType == TemplateType &&
                       string.Equals(Name, typeRegistry.Name, StringComparison.OrdinalIgnoreCase);
            }

            public override int GetHashCode()
            {
                return TemplateType.GetHashCode() + (Name == null ? 0 : Name.GetHashCode());
            }
        }

        internal IRegistry AddRegistry(IRegistry registry)
        {
            _registrations.Add(registry);
            return registry;
        }

        internal IRegistry RegisterType(Type serviceType, ConstructorArgs args, string name = null)
        {
            return AddRegistry(new TypeRegistry(serviceType, args, name));
        }

        internal IRegistry RegisterInstance(object serviceInstance, string name = null)
        {
            return AddRegistry(new InstanceRegistry(serviceInstance, name));
        }

        private class InstanceRegistry: IRegistry, IResolvable
        {
            private readonly object _instance;

            public InstanceRegistry(object instance, string name)
            {
                _instance = instance;
                Name = name;
                ServiceType = _instance.GetType();
            }

            public string Name { get; private set; }

            public IResolvable Resolve(Type targetType)
            {
                if(targetType.IsInstanceOfType(_instance))
                    return this;
                return null;
            }

            public Type ServiceType { get; private set; }

            public object Create(params object[] args)
            {
                return _instance;
            }

            public IEnumerable<IDependency> GetDependencies()
            {
                return Enumerable.Empty<IDependency>();
            }
        }
    }

    public class ConstructorArgs
    {
        public object[] Values { get; set; }

        public ConstructorArgs(params object[] values)
        {
            Values = values;
        }

        public bool Matches(ConstructorInfo constructorInfo)
        {
            if (constructorInfo.GetParameters().Length != this.Values.Length)
                return false;

            var parameters = constructorInfo.GetParameters();
            for (var i = 0; i < Values.Length; i++)
            {
                if (this.Values[i] == null)
                    continue;

                if (!parameters[0].ParameterType.IsInstanceOfType(this.Values[i]))
                    return false;
            }

            return true;
        }
    }

    internal interface IRegistry
    {
        string Name { get; }
        IResolvable Resolve(Type targetType);
    }

}
