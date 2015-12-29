using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Xania.IoC;
using Xania.IoC.Resolvers;

namespace $defaultNamespace$
{
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

    public static class XaniaResolverExtensions
    {
        public static IResolver PerRequest(this IResolver resolver, MvcApplication app)
        {
            return resolver.PerScope(new PerRequestScopeProvider(app));
        }

        public static IResolver PerSession(this IResolver resolver, MvcApplication app)
        {
            return resolver.PerScope(new PerSessionScopeProvider(app));
        }
    }

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

    public class PerSessionScopeProvider : ScopeProvider
    {
        public PerSessionScopeProvider()
            : base(GetBackingStore(Guid.NewGuid().ToString()))
        {
        }

        private static Func<IDictionary<Type, object>> GetBackingStore(string id)
        {
			return () => 
			{
				var session = HttpContext.Current.Session;
				var store = session[id] as IDictionary<Type, object>;
				if (store == null)
					session[id] = store = new Dictionary<Type, object>();
				return store;
			};
        }
    }
}