using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Xania.IoC
{
    public class PerScopeContainer : IObjectContainer, IDisposable
    {
        private readonly IDictionary<Type, IRegistry> _registrations = new Dictionary<Type, IRegistry>();

        public PerScopeContainer Register<TFrom, TTarget>()
            where TTarget: TFrom
        {
            _registrations[typeof(TFrom)] = new TypeRegistry(typeof(TFrom), typeof(TTarget), this);
            return this;
        }

        public PerScopeContainer Register<T>()
        {
            _registrations[typeof(T)] = new TypeRegistry(typeof(T), typeof(T), this);
            return this;
        }

        public virtual object Resolve(Type serviceType)
        {
            IRegistry result;
            if (_registrations.TryGetValue(serviceType, out result))
            {
                return result.Instance;
            }
            return null;
        }

        public virtual void Dispose()
        {
            foreach (var o in _registrations.OfType<IDisposable>())
            {
                o.Dispose();
            }
        }

        private interface IRegistry
        {
            object Instance { get; }
        }

        private class TypeRegistry : IRegistry
        {
            private readonly Type _fromType;
            private readonly Type _targetType;
            private readonly PerScopeContainer _container;
            private bool _resolving = false;
            private object _instance = null;
            private readonly IObjectFactory _objectFactory;

            public TypeRegistry(Type fromType, Type targetType, PerScopeContainer container)
            {
                if (fromType == null)
                    throw new ArgumentNullException("fromType");
                if (targetType == null)
                    throw new ArgumentNullException("targetType");

                _fromType = fromType;
                _targetType = targetType;
                _container = container;
                _objectFactory = new DefaultObjectFactory(container);
            }

            public object Instance
            {
                get
                {
                    if (_instance != null)
                        return _instance;

                    if (_resolving)
                        return null;

                    try
                    {
                        _resolving = true;

                        return _instance = _targetType != _fromType ?
                            _container.Resolve(_targetType) :
                            _objectFactory.Create(_targetType);
                    }
                    finally
                    {
                        _resolving = false;
                    }
                }
            }
        }
    }

    public class ContainerCollection: IObjectContainer, IEnumerable<IObjectContainer>
    {
        private readonly List<IObjectContainer> _items;
        private readonly DefaultObjectFactory _objectFactory;

        public ContainerCollection()
        {
            _items = new List<IObjectContainer>();
            _objectFactory = new DefaultObjectFactory(this);
        }

        public void Add(IObjectContainer c)
        {
            _items.Add(c);
        }

        public object Resolve(Type serviceType)
        {
            return _items.Select(c => c.Resolve(serviceType)).First(_ => _ != null);
        }

        public IEnumerator<IObjectContainer> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}