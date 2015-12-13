using System;

namespace Xania.IoC
{
    public class FactoryResolver : IResolver
    {
        private readonly Func<Type, object> _factory;

        public FactoryResolver(Func<Type, object> factory)
        {
            _factory = factory;
        }

        public IResolvable Resolve(Type type)
        {
            return new InstanceResolvable(_factory(type));
        }
    }
}