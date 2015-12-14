using System;
using Xania.IoC.Resolvers;

namespace Xania.IoC.Containers
{
    public class TransientObjectContainer: IObjectContainer
    {
        private readonly IResolver _resolver;

        public TransientObjectContainer(IResolver resolver)
        {
            _resolver = resolver;
        }

        public TransientObjectContainer(params IResolver[] resolvers)
        {
            _resolver = new ResolverCollection(resolvers);
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