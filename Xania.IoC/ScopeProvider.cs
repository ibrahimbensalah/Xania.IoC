using System;
using System.Collections.Generic;
using System.Linq;

namespace Xania.IoC
{
    public interface IScopeProvider
    {
        IDictionary<Type, object> Get();
    }

    public class ScopeProvider : IScopeProvider
    {
        private Scope _scope;

        public IDictionary<Type, object> Get()
        {
            return _scope ?? (_scope = new Scope());
        }

        internal class Scope : Dictionary<Type, object>
        {
        }
    }
}