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
            return this.SelectMany(r => r.ResolveAll(serviceType)).Where(r => r != null);
        }

        public virtual void Add(IResolver resolver)
        {
            _list.Add(resolver);
        }

        public IEnumerator<IResolver> GetEnumerator()
        {
            for (int i = _list.Count - 1; i >= 0; i--)
            {
                yield return _list[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}