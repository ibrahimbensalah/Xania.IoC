using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Xania.IoC;
using Xania.IoC.Resolvers;

namespace $defaultNamespace$
{
    public class XaniaDependencyResolver : IDependencyResolver
    {
        public XaniaDependencyResolver(MvcApplication mvcApplication)
        {
			PerRequestScopeProvider = new ScopeProvider(() => HttpContext.Current.Items);

            mvcApplication.EndRequest += (sender, args) =>
            {
	            PerRequestScopeProvider.Get().DisposeAll();
            };
        }

		protected virtual IResolver GetPerRequestResolver() 
		{
			return new RegistryResolver()
	            // TODO: register here per request types
				// .Register<DataContext>()
				;
		}

		public IScopeProvider PerRequestScopeProvider { get; private set; }

		private IResolver _resolver;
        public IResolver Resolver
        {
            get
            {
                if (_resolver == null)
                {
                    _resolver = new ResolverCollection
					{
						new ContainerControlledResolver(new ConventionBasedResolver()),
						new IdentityResolver().For<Controller>(),
						GetPerRequestResolver().PerScope(PerRequestScopeProvider),
					};
                }
                return _resolver;
            }
        }

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