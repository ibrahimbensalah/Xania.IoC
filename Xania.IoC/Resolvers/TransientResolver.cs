using System;
using System.Collections.Generic;
using System.Linq;

namespace Xania.IoC.Resolvers
{
    public class TransientResolver : IResolver
    {
        private readonly HashSet<IRegistry> _registrations;

        public TransientResolver()
        {
            _registrations = new HashSet<IRegistry>();
        }

        public virtual IResolvable Resolve(Type type)
        {
            var implementationType = GetImlementationType(type);
            if (implementationType == null)
                return null;

            return TypeResolvable.Create(implementationType);
        }

        public virtual Type GetImlementationType(Type sourceType)
        {
            var registration =
                _registrations.SingleOrDefault(r => r.IsMatch(sourceType));

            if (registration == null)
                return null;

            return registration.ServiceType;
        }

        private interface IRegistry
        {
            bool IsMatch(Type type);

            Type ServiceType { get; }
        }

        private class TypeRegistry: IRegistry
        {
            public TypeRegistry(Type serviceType)
            {
                if (serviceType == null) 
                    throw new ArgumentNullException("serviceType");

                ServiceType = serviceType;
            }

            public Type ServiceType { get; private set; }

            public bool IsMatch(Type type)
            {
                return type.IsAssignableFrom(ServiceType);
            }
        }

        public void Register(Type serviceType)
        {
            _registrations.Add(new TypeRegistry (serviceType));
        }
    }
}