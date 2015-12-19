using System;
using System.Collections.Generic;
using System.Linq;

namespace Xania.IoC
{
    public class ScopeProvider
    {
        private Scope _scope;

        public IDictionary<Type, object> Get()
        {
            return _scope ?? (_scope = new Scope());
        }

        public void Dispose()
        {
            if (_scope != null)
            {
                _scope.Dispose();
                _scope = null;
            }
        }

        internal class Scope : Dictionary<Type, object>, IDisposable
        {
            public void Dispose()
            {
                foreach (var item in Values.OfType<IDisposable>())
                {
                    item.Dispose();
                }
            }
        }
    }
}