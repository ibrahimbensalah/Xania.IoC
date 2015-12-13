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

            return (T) r.Build(resolver);
        }

        public static Resolver Register<TSource>(this Resolver resolver)
        {
            resolver.Register(typeof(TSource));
            return resolver;
        }
    }
}