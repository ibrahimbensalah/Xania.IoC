using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xania.IoC.Containers;
using Xania.IoC.Resolvers;

namespace Xania.IoC
{
	public static class ResolverExtensions
	{
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
			var args = resolvable.GetDependencies().Select(t =>
			{
				object instance;
				try
				{
					instance = resolver.GetService(t);
				}
				catch (ResolutionFailedException ex)
				{
					ex.Types.Add(resolvable.ServiceType);
					throw;
				}
				if (instance == null)
					throw new ResolutionFailedException(t, resolvable.ServiceType);
				return instance;
			}).ToArray();
			return resolvable.Create(args);
		}

		public static RegistryResolver Register<TSource>(this RegistryResolver registryResolver)
		{
			registryResolver.Register(typeof(TSource));
			return registryResolver;
		}

		public static PerScopeResolver PerScope(this IResolver resolver, IScopeProvider scopeProvider)
		{
			return new PerScopeResolver(scopeProvider, resolver);
		}

		public static PerScopeResolver PerScope(this IResolver resolver, Func<IDictionary<Type, object>> backingStore)
		{
		    return resolver.PerScope(new ScopeProvider(backingStore));
		}
	}
}