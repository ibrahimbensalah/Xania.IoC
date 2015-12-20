using System;
using System.Collections;
using System.Collections.Generic;
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
	            ScopeProvider.Get().DisposeAll();
            };
        }

        protected virtual IResolver BuildResolver()
        {
            return new ResolverCollection
            {
                new ContainerControlledResolver(new ConventionBasedResolver()),
                new IdentityResolver().For<Controller>(),
				GetPerRequestResolver().PerScope(PerRequestScopeProvider),
            };
        }

		private IResolver GetPerRequestResolver() 
		{
			return new TransientResolver()
	            // TODO: register here per request types
				// .Register<DataContext>()
				;
		}

		public IScopeProvider PerRequestScopeProvider { get; public set; }

        public IResolver Resolver
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

        public virtual object GetService(Type serviceType)
        {
            return Resolver.Resolve(serviceType).Build(Resolver);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return Enumerable.Empty<object>();
        }
    }
}