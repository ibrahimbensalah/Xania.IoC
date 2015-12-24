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
            return from implementationType in GetImlementationTypes(serviceType)
                   select TypeResolvable.Create(implementationType);
        }

        public virtual IEnumerable<Type> GetImlementationTypes(Type serviceType)
        {
            return
                from registration in _registrations.Where(r => r.IsMatch(serviceType, null))
                select registration.ServiceType;
        }

        private interface IRegistry
        {
            bool IsMatch(Type type, string name);

            Type ServiceType { get; }
        }

        private sealed class TypeRegistry: IRegistry
        {
            private string Name { get; set; }

            public TypeRegistry(Type serviceType, string name)
            {
                if (serviceType == null) 
                    throw new ArgumentNullException("serviceType");
                if (name == null) 
                    throw new ArgumentNullException("name");

                Name = name;
                ServiceType = serviceType;
            }

            public Type ServiceType { get; private set; }

            public bool IsMatch(Type type, string name)
            {
                // type.GetInterfaces(false).Contains(sourceType) || t.BaseType == sourceType;

                return type.IsAssignableFrom(ServiceType) &&
                       string.Equals(Name, name, StringComparison.OrdinalIgnoreCase);
            }

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
