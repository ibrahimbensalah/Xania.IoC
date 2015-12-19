using System;
using System.Collections.Generic;
using System.Linq;

namespace Xania.IoC.Resolvers
{
    public class InstanceResolvable : IResolvable, IDisposable
    {
        private object _instance;

        public InstanceResolvable(object instance)
        {
            _instance = instance;
        }

        public Type ServiceType
        {
            get { return _instance.GetType(); }
        }

        public object Create(params object[] args)
        {
            return _instance;
        }

        public IEnumerable<Type> GetDependencies()
        {
            return Enumerable.Empty<Type>();
        }

        public void Dispose()
        {
            var disposable = _instance as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
            _instance = null;
        }
    }
}