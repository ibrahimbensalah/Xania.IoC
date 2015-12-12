using System;
using System.Reflection;

namespace Xania.IoC
{
    public class TransientContainer : IObjectContainer
    {
        private readonly IObjectFactory _objectFactory;

        public TransientContainer()
        {
            _objectFactory = new DefaultObjectFactory(this);
        }

        public virtual object Resolve(Type serviceType)
        {
            return _objectFactory.Create(serviceType);
        }
    }

    public class CircularReferenceException: Exception
    {
        
    }
}
