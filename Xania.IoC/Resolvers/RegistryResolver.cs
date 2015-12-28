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
            return
                from concreteType in _registrations.Select(r => r.TemplateType.MapTo(serviceType))
                where concreteType != null
                select TypeResolvable.Create(concreteType);
        }

        public virtual IEnumerable<Type> GetImlementationTypes(Type targetType)
        {
            return
                from concreteType in _registrations.Select(r => r.TemplateType.MapTo(targetType))
                where concreteType != null
                select concreteType;
        }

        private interface IRegistry
        {
            string Name { get; }
            Type TemplateType { get; }
        }

        private sealed class TypeRegistry: IRegistry
        {
            public string Name { get; private set; }

            public TypeRegistry(Type templateType, string name)
            {
                if (templateType == null) 
                    throw new ArgumentNullException("templateType");

                Name = name;
                TemplateType = templateType;
            }

            public Type TemplateType { get; private set; }

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
                return typeRegistry.TemplateType == TemplateType &&
                       string.Equals(Name, typeRegistry.Name, StringComparison.OrdinalIgnoreCase);
            }

            public override int GetHashCode()
            {
                return TemplateType.GetHashCode() + (Name == null ? 0 : Name.GetHashCode());
            }
        }

        public void Register(Type serviceType, string name = null)
        {
            _registrations.Add(new TypeRegistry(serviceType, name));
        }
    }
}
