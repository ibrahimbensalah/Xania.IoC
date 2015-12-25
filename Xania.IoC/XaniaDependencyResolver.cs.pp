using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Xania.IoC;
using Xania.IoC.Resolvers;

namespace $defaultNamespace$
{
    public class PerRequestScopeProvider: ScopeProvider
    {
        public PerRequestScopeProvider(MvcApplication mvcApplication)
            : base(() => HttpContext.Current.Items)
        {
            mvcApplication.EndRequest += (sender, args) =>
            {
                Release();
            };
        }
    }

    public class XaniaDependencyResolver : ResolverCollection, IDependencyResolver
    {
		IResolver Resolver { get { return this; } }

        public virtual object GetService(Type serviceType)
        {
            return Resolver.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return Resolver.GetServices(serviceType);
        }
    }
}