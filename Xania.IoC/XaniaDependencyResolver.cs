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
                    _resolver = GetResolver();
                }
                return _resolver;
            }
        }

        protected abstract IResolver GetResolver();

        public void EndRequest(object sender, EventArgs eventArgs)
        {
            Get().DisposeAll();
        }

        public object GetService(Type serviceType)
        {
            return Resolver.Resolve(serviceType).Build(Resolver);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return Enumerable.Empty<object>();
        }
    }
}