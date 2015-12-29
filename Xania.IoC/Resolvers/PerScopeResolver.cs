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

        public int Ha { get; set; }

        public override IEnumerable<IResolvable> ResolveAll(Type serviceType)
        {
            return base.ResolveAll(serviceType)
                .Select(resolvable => new PerScopeResolvable(resolvable.ServiceType, () => GetOrCreate(resolvable)));
        }

        private object GetOrCreate(IResolvable resolvable)
        {
            return _scopeProvider.Get().Get(resolvable.ServiceType, () => this.Build(resolvable));
        }


        public class PerScopeResolvable : IResolvable
        {
            private readonly Func<object> _factory;
            private readonly Type _serviceType;

            public PerScopeResolvable(Type serviceType, Func<object> factory)
            {
                _serviceType = serviceType;
                _factory = factory;
            }

            public Type ServiceType
            {
                get { return _serviceType; }
            }

            public object Create(params object[] args)
            {
                return _factory();
            }

            public IEnumerable<Type> GetDependencies()
            {
                return Enumerable.Empty<Type>();
            }
        }
    }
}