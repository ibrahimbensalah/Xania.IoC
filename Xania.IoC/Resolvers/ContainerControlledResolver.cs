using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xania.IoC.Resolvers
{
    public class ContainerControlledResolver: IResolver
    {
        private readonly IDictionary<Type, InstanceResolvable> _cache = new Dictionary<Type, InstanceResolvable>();
        private readonly IResolver _resolver;

        public ContainerControlledResolver(params IResolver[] resolvers)
        {
            _resolver = new ResolverCollection(resolvers);
        }
        
        /// <summary>
        /// Resolve to InstanceResolver to prevent injecting dependencies from resolvers in higher hierarchy
        /// </summary>
        /// <param name="type">service type to resolve</param>
        /// <returns></returns>
        public IResolvable Resolve(Type type)
        {
            var resolvable = _resolver.Resolve(type);
            if (resolvable == null)
                return null;

            InstanceResolvable instance;
            if (!_cache.TryGetValue(resolvable.ServiceType, out instance))
            {
                instance = new InstanceResolvable(resolvable.ServiceType, resolvable.Build(_resolver));
                _cache.Add(resolvable.ServiceType, instance);
            }

            return instance;
        }
    }
}
