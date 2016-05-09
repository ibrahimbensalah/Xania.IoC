using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Xania.IoC.Resolvers
{
    public class ContainerControlledResolver: ResolverCollection
    {
        // private readonly IDictionary<Type, IEnumerable<StaticResolvable>> _resolvableCache = new Dictionary<Type, IEnumerable<StaticResolvable>>();
        private readonly IDictionary<IResolvable, ContainerControlledResolvable> _resolvableCache = new ConcurrentDictionary<IResolvable, ContainerControlledResolvable>();

        public ContainerControlledResolver(params IResolver[] resolvers)
            : base(resolvers)
        {
        }
        
        /// <summary>
        /// Resolve to StaticResolver to prevent injecting dependencies from resolvers in higher hierarchy
        /// </summary>
        /// <param name="serviceType">service type to resolve</param>
        /// <returns></returns>
        public override IEnumerable<IResolvable> ResolveAll(Type serviceType)
        {
            return 
                from r in base.ResolveAll(serviceType)
                select _resolvableCache.Get(r, () => new ContainerControlledResolvable(r));
        }

        public void Dispose(Type type)
        {
            foreach (var r in base.ResolveAll(type))
            {
                ContainerControlledResolvable x;
                if (_resolvableCache.TryGetValue(r, out x))
                {
                    x.Dispose();
                }
            }
        }
    }
}
