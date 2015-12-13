using System;
using System.Collections.Generic;

namespace Xania.IoC
{
    public class CachedObjectContainer : IObjectContainer
    {
        private readonly IResolver _resolver;

        private readonly IDictionary<Type, object> _cache = new Dictionary<Type, object>();

        public CachedObjectContainer(IResolver resolver)
        {
            _resolver = resolver;
        }

        public object Resolve(Type type)
        {
            object result;
            if (_cache.TryGetValue(type, out result))
                return result;

            var resolvable = _resolver.Resolve(type);
            if (resolvable == null)
                throw new ResolutionFailedException(type);

            result = resolvable.Build(_resolver);
            _cache.Add(type, result);

            return result;
        }
    }
}