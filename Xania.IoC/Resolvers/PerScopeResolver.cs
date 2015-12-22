using System;
using System.Collections.Generic;
using System.Linq;

namespace Xania.IoC.Resolvers
{
    public class PerScopeResolver : IResolver
    {
        private readonly IResolver _resolver;
        private readonly IScopeProvider _scopeProvider;

        public PerScopeResolver(IScopeProvider scopeProvider, IResolver resolver)
        {
            _scopeProvider = scopeProvider;
            _resolver = resolver;
        }

        public PerScopeResolver(IScopeProvider scopeProvider, params IResolver[] resolvers)
            : this(scopeProvider, new ResolverCollection(resolvers))
        {
        }

        public virtual IEnumerable<IResolvable> ResolveAll(Type type)
        {
            return 
                from resolvable in _resolver.ResolveAll(type)
                select new PerScopeResolvable(type, resolvable, _scopeProvider);
        }


        public class PerScopeResolvable : IResolvable
        {
            private readonly Type _type;
            private readonly IResolvable _resolvable;
            private readonly IScopeProvider _scopeProvider;

            public PerScopeResolvable(Type type, IResolvable resolvable, IScopeProvider scopeProvider)
            {
                _type = type;
                _resolvable = resolvable;
                _scopeProvider = scopeProvider;
            }

            public Type ServiceType
            {
                get { return _resolvable.ServiceType; }
            }

            public object Create(params object[] args)
            {
                return new ScopeDecoraptor(_type, () => _resolvable.Create(args), _scopeProvider).GetTransparentProxy();
            }

            public IEnumerable<Type> GetDependencies()
            {
                return _resolvable.GetDependencies();
            }
        }
    }
}