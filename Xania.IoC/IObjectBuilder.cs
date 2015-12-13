using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Xania.IoC
{
    public class PerTypeContainer : IObjectContainer
    {
        private readonly IResolver _resolver;

        private readonly IDictionary<Type, object> _cache = new Dictionary<Type, object>();

        public PerTypeContainer(IResolver resolver)
        {
            _resolver = resolver;
        }

        public object Resolve(Type type)
        {
            object result;
            if (_cache.TryGetValue(type, out result))
                return result;

            var resolvable = _resolver.Resolve(type) ?? TypeResolvable.Create(type);
            result = resolvable.Build(_resolver);
            _cache.Add(type, result);

            return result;
        }
    }
}