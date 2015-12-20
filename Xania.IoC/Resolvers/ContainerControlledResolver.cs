using System;
using System.Collections.Generic;
using System.Linq;

namespace Xania.IoC.Resolvers
{
    public class ContainerControlledResolver: IResolver
    {
        private readonly IDictionary<Type, InstanceResolvable> _resolvableCache = new Dictionary<Type, InstanceResolvable>();
        private readonly IResolver _resolver;

        public ContainerControlledResolver(params IResolver[] resolvers)
        {
            _resolver = new ResolverCollection(resolvers);
        }
        
        /// <summary>
        /// Resolve to InstanceResolver to prevent injecting dependencies from resolvers in higher hierarchy
        /// </summary>
        /// <param name="type">service type to resolve</param>
        /// <returns></returns>
        public IResolvable Resolve(Type type)
        {
            var observable = _resolver.Resolve(type);
            if (observable == null)
                return null;

            return _resolvableCache.Get(type, () => new InstanceResolvable(observable));
        }

        public void Dispose(Type type)
        {
            _resolvableCache.Dispose(type);
        }

        public class InstanceResolvable : IResolvable, IDisposable
        {
            private readonly IResolvable _resolvable;
            private object _instance;

            public InstanceResolvable(IResolvable resolvable)
            {
                _resolvable = resolvable;
            }

            public Type ServiceType
            {
                get { return _instance.GetType(); }
            }

            public object Create(params object[] args)
            {
                return _instance ?? (_instance = _resolvable.Create(args));
            }

            public IEnumerable<Type> GetDependencies()
            {
                return _resolvable.GetDependencies();
            }

            public void Dispose()
            {
                var disposable = _instance as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
                _instance = null;
            }
        }
    }
}
