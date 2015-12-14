using System;
using System.Linq;
using Xania.IoC.Resolvers;

namespace Xania.IoC.Containers
{
    public class ObjectContainer: IObjectContainer
    {
        private readonly IResolver[] _resolvers;

        public ObjectContainer(params IResolver[] resolvers)
        {
            _resolvers = resolvers;
        }

        public object Resolve(Type serviceType)
        {
            var result = _resolvers.Select(resolver => Resolve(serviceType, resolver)).FirstOrDefault(x => x != null);
            if (result == null)
                throw new ResolutionFailedException(serviceType);

            return result;
        }

        private object Resolve(Type type, IResolver resolver)
        {
            var resolvable = resolver.Resolve(type);
            if (resolvable == null)
                return null;

            return resolvable.Build(resolver);
        }
    }
}