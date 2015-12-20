using System;
using System.Collections.Generic;

namespace Xania.IoC.Resolvers
{
    public class ContainerControlledResolver: IResolver
    {
        private readonly IDictionary<Type, InstanceResolvable> _resolvableCache = new Dictionary<Type, InstanceResolvable>();
        private readonly IResolver _resolver;
        public Dictionary<Type, IScopeProvider> ScopeProviders { get; private set; }

        public ContainerControlledResolver(params IResolver[] resolvers)
        {
            _resolver = new ResolverCollection(resolvers);
            ScopeProviders = new Dictionary<Type, IScopeProvider>();
        }
        
        /// <summary>
        /// Resolve to InstanceResolver to prevent injecting dependencies from resolvers in higher hierarchy
        /// </summary>
        /// <param name="type">service type to resolve</param>
        /// <returns></returns>
        public IResolvable Resolve(Type type)
        {
            return _resolvableCache.Get(type, () => new InstanceResolvable(Build(type)));
        }

        private object Build(Type type)
        {
            var observable = _resolver.Resolve(type);
            if (observable == null)
                return null;

            IScopeProvider scopeProvider;
            if (ScopeProviders.TryGetValue(type, out scopeProvider))
            {
                return new ScopeDecoraptor(type, () => _resolver.Build(observable), scopeProvider).GetTransparentProxy();
            }
            return _resolver.Build(observable);
        }

        public void Dispose(Type type)
        {
            _resolvableCache.Dispose(type);
        }
    }
}
