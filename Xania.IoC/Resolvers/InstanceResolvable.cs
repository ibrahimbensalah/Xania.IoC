using System;
using System.Collections.Generic;
using System.Linq;

namespace Xania.IoC.Resolvers
{
    public class InstanceResolvable : IResolvable
    {
        private readonly IResolvable _resolvable;
        private readonly IResolver _resolver;
        private object _instance;

        public InstanceResolvable(IResolvable resolvable, IResolver resolver)
        {
            _resolvable = resolvable;
            _resolver = resolver;
        }

        public Type ServiceType
        {
            get { return _resolvable.ServiceType; }
        }

        public object Create(params object[] args)
        {
            return _instance ?? (_instance = _resolver.Build(_resolvable));
        }

        public IEnumerable<Type> GetDependencies()
        {
            return Enumerable.Empty<Type>();
        }

        public void Dispose()
        {
            if (_instance is IDisposable)
            {
                var disposable = _instance as IDisposable;
                disposable.Dispose();
                _instance = null;
            }
        }
    }
}