using System;
using System.Collections;
using System.Collections.Generic;
using Xania.IoC.Resolvers;

namespace Xania.IoC
{
    public interface IScopeProvider
    {
        void Release();

        IDictionary<Type, object> Get();
    }

    public class ScopeProvider : IScopeProvider
    {
        private readonly Func<IDictionary<Type, object>> _typeStore;

        public ScopeProvider()
            : this(new Dictionary<Type, object>())
        {
        }

        public ScopeProvider(IDictionary<Type, object> typeStore)
            : this(() => typeStore)
        {
        }

        public ScopeProvider(Func<IDictionary<Type, object>> typeStore)
        {
            _typeStore = typeStore;
        }

        public virtual void Release()
        {
            _typeStore().DisposeAll();
        }

        public virtual IDictionary<Type, object> Get()
        {
            return _typeStore();
        }

        public static IScopeProvider FromBackingStore(Func<IDictionary> backingStore)
        {
            return new ScopeProvider(GetTypeStore(backingStore, Guid.NewGuid()));
        }

        private static Func<IDictionary<Type, object>> GetTypeStore(Func<IDictionary> backingStore, Guid id)
        {
            return () =>
            {
                var dictionary = backingStore();
                var store = dictionary[id] as IDictionary<Type, object>;
                if (store == null)
                    dictionary[id] = (store = new Dictionary<Type, object>());
                return store;
            };
        }
    }
}