using System;
using System.Collections.Generic;

namespace Xania.IoC.Resolvers
{
    public class InstanceResolvable : IResolvable, IDisposable
    {
        private readonly IResolvable _resolvable;
        private object _instance;

        public InstanceResolvable(IResolvable resolvable)
        {
            _resolvable = resolvable;
        }

        public Type ServiceType
        {
            get { return _instance.GetType(); }
        }

        public object Create(params object[] args)
        {
            return _instance ?? (_instance = _resolvable.Create(args));
        }

        public IEnumerable<Type> GetDependencies()
        {
            return _resolvable.GetDependencies();
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