using System;
using System.Linq;
using Xania.IoC.Containers;
using Xania.IoC.Resolvers;

namespace Xania.IoC
{
    public static class ResolverExtensions
    {
        public static T Resolve<T>(this IObjectContainer container)
        {
            return (T) container.Resolve(typeof (T));
        }

        public static T Resolve<T>(this IResolver resolver)
        {
            var r = resolver.Resolve(typeof (T));
            if (r == null)
                throw new ResolutionFailedException(typeof(T));

            return (T) resolver.Build(r);
        }

        public static object Build(this IResolver resolver, IResolvable resolvable)
        {
            if (resolvable == null)
                return null;

            var args = resolvable.GetDependencies().Select(d =>
            {
                var r = resolver.Resolve(d);
                if (r == null)
                    throw new ResolutionFailedException(d);
                return resolver.Build(r);
            });
            return resolvable.Create(args.ToArray());
        }

        public static TransientResolver Register<TSource>(this TransientResolver transientResolver)
        {
            transientResolver.Register(typeof(TSource));
            return transientResolver;
        }
    }
}