using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Xania.IoC
{
    public class ResolverCollection : IResolver, IEnumerable<IResolver>
    {
        private readonly List<IResolver> _list;

        public ResolverCollection()
        {
            _list = new List<IResolver>();
        }

        public IResolvable Resolve(Type type)
        {
            return _list.Select(r => r.Resolve(type)).LastOrDefault(r => r != null);
        }

        public void Add(IResolver resolver)
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