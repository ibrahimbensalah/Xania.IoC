using System;
using System.Collections.Generic;

namespace Xania.IoC.Resolvers
{
    public class ContainerControlledResolvable : IResolvable, IDisposable
    {
        private readonly IResolvable _resolvable;
        private object _instance;

        public ContainerControlledResolvable(IResolvable resolvable)
        {
            _resolvable = resolvable;
        }

        public Type ServiceType
        {
            get { return _resolvable.ServiceType; }
        }

        public object Create(params object[] args)
        {
            return _instance ?? (_instance = _resolvable.Create(args));
        }

        public IEnumerable<IDependency> GetDependencies()
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