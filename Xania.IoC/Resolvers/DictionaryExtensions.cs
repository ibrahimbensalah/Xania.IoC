using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Xania.IoC.Resolvers
{
    public static class DictionaryExtensions
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

        public static void DisposeAll<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, bool clearAll = true)
        {
            if (dictionary != null)
            {
                foreach (var item in dictionary.Values.OfType<IDisposable>())
                    item.Dispose();

                if (clearAll)
                    dictionary.Clear();
            }
        }

        public static void Dispose(this object instance)
        {
            var disposable = instance as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }
    }
}