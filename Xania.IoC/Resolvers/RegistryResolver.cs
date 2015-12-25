using System;
using System.Collections.Generic;
using System.Linq;

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
            var implementationTypes = GetImlementationTypes(serviceType).ToArray();
            return from implementationType in implementationTypes
                   select TypeResolvable.Create(implementationType);
        }

        public virtual IEnumerable<Type> GetImlementationTypes(Type serviceType)
        {
            return
                from registration in _registrations
                let concreteType = GetConcreteType(registration.ServiceType, serviceType)
                where concreteType != null
                select concreteType;
        }

        private Type GetConcreteType(Type serviceType, Type baseType)
        {
            if (serviceType.ContainsGenericParameters)
                serviceType = serviceType.MakeGenericType(baseType.GenericTypeArguments);

            if (baseType.IsAssignableFrom(serviceType))
                return serviceType;

            return null;
        }

        private interface IRegistry
        {
            string Name { get; }
            Type ServiceType { get; }
        }

        private sealed class TypeRegistry: IRegistry
        {
            public string Name { get; private set; }

            public TypeRegistry(Type serviceType, string name)
            {
                if (serviceType == null) 
                    throw new ArgumentNullException("serviceType");

                Name = name;
                ServiceType = serviceType;
            }

            public Type ServiceType { get; private set; }

            // override object.Equals
            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }

                return Equals(obj as TypeRegistry);
                
            }

            public bool Equals(TypeRegistry typeRegistry)
            {
                return typeRegistry.ServiceType == ServiceType &&
                       string.Equals(Name, typeRegistry.Name, StringComparison.OrdinalIgnoreCase);
            }

            public override int GetHashCode()
            {
                return ServiceType.GetHashCode() + (Name == null ? 0 : Name.GetHashCode());
            }
        }

        public void Register(Type serviceType, string name = null)
        {
            _registrations.Add(new TypeRegistry(serviceType, name));
        }
    }
}
