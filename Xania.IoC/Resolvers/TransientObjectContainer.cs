using System;

namespace Xania.IoC.Resolvers
{
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