using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

    public class TransientObjectContainer: IObjectContainer
    {
        private readonly IResolver _resolver;

        public TransientObjectContainer(IResolver resolver)
        {
            _resolver = resolver;
        }

        public object Resolve(Type serviceType)
        {
            var resolvable = _resolver.Resolve(serviceType);
            if (resolvable == null)
                throw new ResolutionFailedException(serviceType);

            return resolvable.Build(_resolver);
        }
    }
}