using System;

namespace Xania.IoC.Resolvers
{
    public class InstanceResolvable : IResolvable
    {
        private readonly object _instance;

        public InstanceResolvable(Type type, object instance)
        {
            ServiceType = type;
            _instance = instance;
        }

        public object Create(params object[] args)
        {
            return _instance;
        }

        public object Build(IResolver resolver)
        {
            return _instance;
        }

        public Type ServiceType { get; private set; }
    }
}