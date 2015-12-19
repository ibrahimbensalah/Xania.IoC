using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xania.IoC.Resolvers;

namespace Xania.IoC
{
    public class TypeResolvable: IResolvable
    {
        private readonly ConstructorInfo _ctor;

        public TypeResolvable(Type serviceType, ConstructorInfo ctor)
        {
            _ctor = ctor;
            ServiceType = serviceType;
        }

        public IEnumerable<Type> GetDependencies()
        {
            return _ctor.GetParameters().Select(p => p.ParameterType);
        }

        public object Create(params object[] args)
        {
            if (args.Any(x => x == null))
                return null;

            return _ctor.Invoke(args);
        }

        //public object Build(IResolver resolver)
        //{
        //    var instance = _ctors.Select(ctor => Build(ctor, resolver)).FirstOrDefault(e => e != null);

        //    if (instance == null)
        //        throw new ResolutionFailedException(ServiceType);

        //    return instance;
        //}

        //public object Build(ConstructorInfo ctor, IResolver resolver)
        //{
        //    var args = GetDependencies(ctor).Select(d =>
        //    {
        //        var r = resolver.Resolve(d);
        //        if (r == null)
        //            throw new ResolutionFailedException(d);
        //        return r.Build(resolver);
        //    });
        //    return Create(ctor, args.ToArray());
        //}

        public Type ServiceType { get; private set; }

        public static TypeResolvable Create(Type implementationType)
        {
            var ctor = implementationType
                .GetConstructors()
                .OrderByDescending(e => e.GetParameters().Length)
                .FirstOrDefault();

            return new TypeResolvable(implementationType, ctor);
        }
    }
}