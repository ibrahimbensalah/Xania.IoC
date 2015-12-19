using System;
using System.Collections.Generic;

namespace Xania.IoC.Resolvers
{
    public class ContainerControlledResolver: IResolver
    {
        private readonly IDictionary<Type, InstanceResolvable> _resolvableCache = new Dictionary<Type, InstanceResolvable>();
        private readonly IResolver _resolver;

        public ContainerControlledResolver(params IResolver[] resolvers)
        {
            _resolver = new ResolverCollection(resolvers);
        }
        
        /// <summary>
        /// Resolve to InstanceResolver to prevent injecting dependencies from resolvers in higher hierarchy
        /// </summary>
        /// <param name="type">service type to resolve</param>
        /// <returns></returns>
        public IResolvable Resolve(Type type)
        {
            return _resolvableCache.Get(type, () => new InstanceResolvable(_resolver.Resolve(type), _resolver));
        }

        public void Dispose(Type type)
        {
            _resolvableCache.Dispose(type);
        }
    }

    internal static class DictionaryExtensions
    {
        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> cache, TKey key, Func<TValue> factory)
        {
            TValue value;
            if (!cache.TryGetValue(key, out value))
                cache.Add(key, value = factory());
            return value;
        }

        public static bool Dispose<TKey, TValue>(this IDictionary<TKey, TValue> cache, TKey key)
        {
            TValue value;
            if (!cache.TryGetValue(key, out value)) 
                return false;

            cache.Remove(key);

            var disp = value as IDisposable;
            if (disp != null)
                disp.Dispose();

            return true;
        }
    }
}
