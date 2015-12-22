using System;
using System.Linq;
using Xania.IoC.Resolvers;

namespace Xania.IoC.Containers
{
    //public class ObjectContainer : IObjectContainer
    //{
    //    private readonly IResolver[] _resolvers;

    //    public ObjectContainer(params IResolver[] resolvers)
    //    {
    //        _resolvers = resolvers;
    //    }

    //    //public object Resolve(Type serviceType)
    //    //{
    //    //    return _resolvers
    //    //        .Select(resolver => Resolve(serviceType, resolver))
    //    //        .FirstOrDefault(x => x != null);
    //    //}

    //    //private object Resolve(Type type, IResolver resolver)
    //    //{
    //    //    var resolvable = resolver.Resolve(type);
    //    //    if (resolvable == null)
    //    //        return null;

    //    //    return resolver.Build(resolvable);
    //    //}
    //}
}