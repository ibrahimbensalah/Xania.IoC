using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xania.IoC.Resolvers;

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

        public virtual void Release()
        {
            var backingStore = _backingStore();
            if (!backingStore.Contains(_id))
            {
                var items = (IDictionary<Type, object>)backingStore[_id];
                items.DisposeAll();
            }
        }

        public virtual IDictionary<Type, object> Get()
        {
            var backingStore = _backingStore();
            if (!backingStore.Contains(_id))
            {
                backingStore[_id] = new Dictionary<Type, object>();
            }

            return (IDictionary<Type, object>)backingStore[_id];
        }
    }
}