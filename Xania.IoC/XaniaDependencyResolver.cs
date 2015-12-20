using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xania.IoC.Resolvers;

namespace Xania.IoC
{
    public abstract class XaniaDependencyResolver : ScopeProvider
    {
        private IResolver _resolver;

        protected XaniaDependencyResolver(Func<IDictionary> itemsFunc)
            : base(itemsFunc)
        {
        }

        private IResolver Resolver
        {
            get
            {
                if (_resolver == null)
                {
                    _resolver = BuildResolver();
                }
                return _resolver;
            }
        }

        protected abstract IResolver BuildResolver();

        protected void OnEndRequest()
        {
            Get().DisposeAll();
        }

        public virtual object GetService(Type serviceType)
        {
            return Resolver.Resolve(serviceType).Build(Resolver);
        }

        public virtual IEnumerable<object> GetServices(Type serviceType)
        {
            return new[] {GetService(serviceType)};
        }
    }
}