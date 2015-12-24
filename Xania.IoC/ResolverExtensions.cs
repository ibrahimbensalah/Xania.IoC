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
        public static T Resolve<T>(this IResolver resolver)
        {
            return (T)resolver.Resolve(typeof(T));
        }

        public static IEnumerable<T> ResolveAll<T>(this IResolver resolver)
        {
            return resolver.ResolveAll(typeof(T)).Cast<T>();
        }

        public static object Resolve(this IResolver resolver, Type serviceType)
        {
            var resolvables = resolver.ResolveAll(serviceType).Select(res => res.Build(resolver)).Take(2).ToArray();
            switch (resolvables.Count())
            {
                case 1:
                    return resolvables[0];
                default:
                    throw new ResolutionFailedException(serviceType);
            }
        }

        public static object Build(this IResolvable resolvable, IResolver resolver)
        {
            return resolver.Build(resolvable);
        }

        public static object Build(this IResolver resolver, IResolvable resolvable)
        {
            if (resolvable == null)
                return null;

            var args = resolvable.GetDependencies().Select(resolver.Resolve).ToArray();

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

        public static PerScopeResolver PerScope(this IResolver resolver, Func<IDictionary> backingStore)
        {
            var scopeProvider = new ScopeProvider(backingStore);
            return new PerScopeResolver(scopeProvider, resolver);
        }
    }
}