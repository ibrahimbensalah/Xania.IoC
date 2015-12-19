using System;
using System.Collections.Generic;

namespace Xania.IoC.Resolvers
{
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