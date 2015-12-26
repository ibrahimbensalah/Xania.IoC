using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xania.IoC
{
	public static class CachingExtensions
	{
		private static IDictionary<CacheKey, object> _values = new Dictionary<CacheKey, object>();

		public static TValue CacheGet<TKey, TValue>(this TKey key, Func<TValue> factory)
		{
			CacheKey cacheKey = new CacheKey(key);
			object value;
			if (!_values.TryGetValue(cacheKey, out value))
			{
				value = factory();
				if (value == null)
					throw new ArgumentNullException("factory");
				_values.Add(cacheKey, value);
			}
			return (TValue) value;
		}

		private class CacheKey
		{
			private object _key;
			public CacheKey(object key)
			{
				if (key == null)
					throw new ArgumentNullException("key");

				_key = key;
			}

			public override bool Equals(object obj)
			{
				if (obj is CacheKey)
					return Equals(obj as CacheKey);
				return 
					false;
			}

			private bool Equals(CacheKey cacheKey)
			{
				return Equals(_key, cacheKey._key);
			}

			public override int GetHashCode()
			{
				return _key.GetHashCode();
			}
		}
	}
}
