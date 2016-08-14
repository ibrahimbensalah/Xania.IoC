using System;
using System.Collections.Generic;
using System.Linq;
using Xania.IoC.Resolvers;

namespace Xania.IoC
{
	public static class ResolverExtensions
	{
        public static IResolvable Resolve<T>(this IResolver resolver)
        {
            return resolver.Resolve(typeof(T));
        }

        public static IResolvable Resolve(this IResolver resolver, Type serviceType)
        {
            return resolver.ResolveAll(serviceType).FirstOrDefault();
        }

        public static T GetService<T>(this IResolver resolver)
        {
            return (T)resolver.GetService(typeof(T));
        }

        public static object GetService(this IResolver resolver, Type serviceType)
		{
			return resolver.GetServices(serviceType).FirstOrDefault();
		}

		public static IEnumerable<T> GetServices<T>(this IResolver resolver)
		{
			return resolver.GetServices(typeof(T)).Cast<T>();
		}

		public static IEnumerable<object> GetServices(this IResolver resolver, Type serviceType)
		{
			return resolver.ResolveAll(serviceType).Select(resolver.Build);
		}

		public static object Build(this IResolvable resolvable, IResolver resolver)
		{
			return resolver.Build(resolvable);
		}

		public static object Build(this IResolver resolver, IResolvable resolvable)
		{
		    var args = BuildDependencies(resolver, resolvable).ToArray();
		    return resolvable.Create(args);
		}

	    private static IEnumerable<object> BuildDependencies(IResolver resolver, IResolvable resolvable)
	    {
	        return resolvable.GetDependencies().Select(t =>
	        {
	            object instance = null;
	            try
	            {
	                instance = t.Build(resolver);
	            }
	            catch (ResolutionFailedException ex)
	            {
	                ex.Types.Add(resolvable.ServiceType);
	                throw;
	            }
	            if (instance == null)
	                throw new ResolutionFailedException(t, resolvable.ServiceType);
	            return instance;
	        });
	    }

        public static RegistryResolver Register<TService>(this RegistryResolver registryResolver, ConstructorArgs args = null)
        {
            registryResolver.RegisterType(typeof(TService), args);
            return registryResolver;
        }

        public static RegistryResolver Register(this RegistryResolver registryResolver, Type serviceType, ConstructorArgs args = null)
        {
            registryResolver.RegisterType(serviceType, args);
            return registryResolver;
        }

        public static RegistryResolver Register<TService>(this RegistryResolver registryResolver, TService instance)
        {
            registryResolver.RegisterInstance(instance);
            return registryResolver;
        }

        public static PerScopeResolver PerScope(this IResolver resolver, IScopeProvider scopeProvider)
		{
            return new PerScopeResolver(scopeProvider) { resolver };
		}

		public static PerScopeResolver PerScope(this IResolver resolver, Func<IDictionary<Type, object>> backingStore)
		{
		    return resolver.PerScope(new ScopeProvider(backingStore));
		}
	}
}