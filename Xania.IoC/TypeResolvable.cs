using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xania.IoC.Resolvers;

namespace Xania.IoC
{
    public class TypeResolvable: IResolvable
    {
        public ConstructorInfo Ctor { get; set; }

        public TypeResolvable(ConstructorInfo ctor)
        {
            Ctor = ctor;
        }

        public IEnumerable<Type> GetDependencies()
        {
            return Ctor.GetParameters().Select(p => p.ParameterType);
        }

        public object Create(object[] args)
        {
            if (args.Any(x => x == null))
                throw new ResolutionFailedException(Ctor.DeclaringType);

            return Ctor.Invoke(args);
        }

        public object Build(IResolver resolver)
        {
            var args = GetDependencies().Select(d =>
            {
                var r = resolver.Resolve(d);
                if (r == null)
                    throw new ResolutionFailedException(d);
                return r.Build(resolver);
            });
            return Create(args.ToArray());
        }

        public Type ServiceType { get { return Ctor.DeclaringType; } }

        public static TypeResolvable Create(Type implementationType)
        {
            var ctor = implementationType
                .GetConstructors()
                .OrderByDescending(e => e.GetParameters().Length)
                .FirstOrDefault();

            return new TypeResolvable(ctor);
        }
    }

    //public class TypeResolvable : IResolvable
    //{
    //    private readonly Type _type;

    //    public TypeResolvable(Type type)
    //    {
    //        _type = type;
    //    }

    //    public object Build(IResolver resolver)
    //    {
    //        var resolvable = resolver.Resolve(_type);
    //        if (resolvable == null)
    //            throw new ResolutionFailedException(_type);

    //        return resolvable.Build(resolver);
    //    }
    //}
}