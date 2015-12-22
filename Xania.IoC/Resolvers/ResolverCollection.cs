﻿using System;
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

        public IEnumerable<IResolvable> ResolveAll(Type type)
        {
            return _list.SelectMany(r => r.ResolveAll(type)).Where(r => r != null);
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