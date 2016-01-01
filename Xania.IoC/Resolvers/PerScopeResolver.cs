using System;
using System.Collections.Generic;
using System.Linq;

namespace Xania.IoC.Resolvers
{
    public class PerScopeResolver : ResolverCollection
    {
        private readonly IScopeProvider _scopeProvider;

        public PerScopeResolver()
            : this(new ScopeProvider())
        {
        }

        public PerScopeResolver(IScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        public ResolverCollection Resolvers
        {
            get { return this; }
        }

        public override IEnumerable<IResolvable> ResolveAll(Type serviceType)
        {
            return base.ResolveAll(serviceType)
                .Select(resolvable => new PerScopeResolvable(resolvable, this));
        }

        private IDictionary<Type, object> TypeStore
        {
            get { return _scopeProvider.Get(); }
        }

        public class PerScopeResolvable : IResolvable
        {
            private readonly IResolvable _resolvable;
            private readonly PerScopeResolver _perScopeResolver;

            public PerScopeResolvable(IResolvable resolvable, PerScopeResolver perScopeResolver)
            {
                if (resolvable == null) 
                    throw new ArgumentNullException("resolvable");

                _resolvable = resolvable;
                _perScopeResolver = perScopeResolver;
            }

            public Type ServiceType
            {
                get { return _resolvable.ServiceType; }
            }

            public object Create(params object[] args)
            {
                return GetOrCreate();
            }

            public IEnumerable<IDependency> GetDependencies()
            {
                return Enumerable.Empty<IDependency>();
            }

            private object GetOrCreate()
            {
                return _perScopeResolver.TypeStore.Get(_resolvable.ServiceType, () => _perScopeResolver.Build(_resolvable));
            }
        }
    }
}