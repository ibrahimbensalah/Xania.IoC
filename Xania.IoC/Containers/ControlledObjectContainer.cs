using System;
using System.Collections.Generic;
using Xania.IoC.Resolvers;

namespace Xania.IoC.Containers
{
    public class ControlledObjectContainer : IObjectContainer
    {
        private readonly IResolver _resolver;

        private readonly IDictionary<Type, object> _cache = new Dictionary<Type, object>();

        public ControlledObjectContainer(IResolver resolver)
        {
            _resolver = resolver;
        }

        public ControlledObjectContainer(params IResolver[] resolvers)
        {
            _resolver = new ResolverCollection(resolvers);
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