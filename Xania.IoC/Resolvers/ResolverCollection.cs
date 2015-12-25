using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Xania.IoC.Resolvers
{
    public class ResolverCollection : IResolver, IEnumerable<IResolver>
    {
        private readonly List<IResolver> _list;

        public ResolverCollection(params IResolver[] resolvers)
        {
            _list = new List<IResolver>(resolvers);
        }

        public virtual IEnumerable<IResolvable> ResolveAll(Type serviceType)
        {
            return _list.SelectMany(r => r.ResolveAll(serviceType)).Where(r => r != null);
        }

        public virtual void Add(IResolver resolver)
        {
            _list.Add(resolver);
        }

        public IEnumerator<IResolver> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}