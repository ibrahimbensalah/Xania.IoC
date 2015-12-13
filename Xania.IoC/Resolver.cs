using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Xania.IoC
{
    public class Resolver : IResolver
    {
        private readonly HashSet<IRegistry> _registrations;

        public Resolver()
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

            return registration.TargetType;
        }

        private interface IRegistry
        {
            bool IsMatch(Type type);
            Type TargetType { get; }
            Type SourceType { get; }
        }

        private class TypeRegistry: IRegistry
        {
            public TypeRegistry(Type sourceType, Type targetType = null)
            {
                if (sourceType == null) 
                    throw new ArgumentNullException("sourceType");

                SourceType = sourceType;
                TargetType = targetType ?? sourceType;
            }

            public Type SourceType { get; private set; }
            public Type TargetType { get; private set; }
            public bool IsMatch(Type type)
            {
                return SourceType == type;
            }
        }

        public void Register(Type sourceType, Type targetType)
        {
            _registrations.Add(new TypeRegistry (sourceType, targetType));
        }
    }

    public class ResolverCollection : IResolver, IEnumerable<IResolver>
    {
        private readonly List<IResolver> _list;

        public ResolverCollection()
        {
            _list = new List<IResolver>();
        }

        public IResolvable Resolve(Type type)
        {
            return _list.Select(r => r.Resolve(type)).FirstOrDefault(r => r != null);
        }

        public void Add(IResolver resolver)
        {
            _list.Add(resolver);
        }

        public IEnumerator<IResolver> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class InstanceResolver : IResolver
    {
        private readonly Func<Type, object> _factory;

        public InstanceResolver(Func<Type, object> factory)
        {
            _factory = factory;
        }

        public IResolvable Resolve(Type type)
        {
            return new InstanceResolvable(_factory(type));
        }
    }

    public class InstanceResolvable : IResolvable
    {
        private readonly object _instance;

        public InstanceResolvable(object instance)
        {
            _instance = instance;
        }

        public object Create(params object[] args)
        {
            return _instance;
        }

        public object Build(IResolver resolver)
        {
            return _instance;
        }

        public IEnumerable<Type> GetDependencies()
        {
            return Enumerable.Empty<Type>();
        }
    }

    public class FactoryResolver : IResolver
    {
        private readonly Func<Type, IResolvable> _factory;

        public FactoryResolver(Func<Type, IResolvable> factory)
        {
            _factory = factory;
        }

        public IResolvable Resolve(Type type)
        {
            return _factory(type);
        }
    }
}