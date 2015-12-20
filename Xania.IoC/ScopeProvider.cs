using System;
using System.Collections;
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
        private readonly Func<IDictionary> _backingStore;
        private readonly Guid _id;

        public ScopeProvider()
            : this(new Hashtable())
        {
        }

        public ScopeProvider(IDictionary backingStore)
            : this(() => backingStore)
        {
        }

        public ScopeProvider(Func<IDictionary> backingStore)
        {
            _backingStore = backingStore;
            _id = Guid.NewGuid();
        }

        public IDictionary<Type, object> Get()
        {
            var items = _backingStore();
            if (!items.Contains(_id))
            {
                items[_id] = new Dictionary<Type, object>();
            }

            return (IDictionary<Type, object>)items[_id];
        }
    }
}