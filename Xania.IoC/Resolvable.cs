using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Xania.IoC
{
    public class Resolvable
    {
        public ConstructorInfo Ctor { get; set; }

        public Resolvable(ConstructorInfo ctor)
        {
            Ctor = ctor;
        }

        public IEnumerable<Type> GetDependencies()
        {
            return Ctor.GetParameters().Select(p => p.ParameterType);
        }

        public object Build(object[] args)
        {
            if (args.Any(x => x == null))
                throw new ResolutionFailedException(this);

            return Ctor.Invoke(args);
        }

        public virtual object Build(IResolver resolver)
        {
            var args = GetDependencies().Select(t => resolver.Resolve(t).Build(resolver)).ToArray();
            return Build(args);
        }
    }

    public interface IResolver
    {
        Resolvable Resolve(Type type);
    }

    public class Resolver : IResolver
    {
        public virtual Resolvable Resolve(Type type)
        {
            var ctor = type.GetConstructors().First();
            return new Resolvable(ctor);
        }
    }

    public static class ResolverExtensions
    {
        public static T Resolve<T>(this IResolver resolver)
        {
            return (T) resolver.Resolve(typeof (T)).Build(resolver);
        }
    }
}