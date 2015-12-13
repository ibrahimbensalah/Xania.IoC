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
            var args = GetDependencies().Select(d =>
            {
                var r = resolver.Resolve(d);
                if (r == null)
                    throw  new ResolutionFailedException(d);
                return r.Build(resolver);
            });
            return Build(args.ToArray());
        }
    }

    public interface IResolver
    {
        Resolvable Resolve(Type type);
    }

    public class Resolver : IResolver
    {
        private readonly HashSet<IRegistry> _registrations;

        public Resolver()
        {
            _registrations = new HashSet<IRegistry>();
        }

        public virtual Resolvable Resolve(Type type)
        {
            var implementationType = GetImlementationType(type);
            if (implementationType == null)
                return null;

            if (implementationType.IsAbstract)
                throw new ResolutionFailedException(implementationType);
            var ctor = implementationType.GetConstructors().FirstOrDefault(c => c.IsPublic);
            if (ctor == null)
                throw new ResolutionFailedException(implementationType);

            return new Resolvable(ctor);
        }

        public virtual Type GetImlementationType(Type sourceType)
        {
            var registration =
                _registrations.SingleOrDefault(r => r.IsMatch(sourceType));

            if (registration == null)
                return null;

            return registration.TargetType;
        }

        private interface IRegistry
        {
            bool IsMatch(Type type);
            Type TargetType { get; }
            Type SourceType { get; }
        }

        private class TypeRegistry: IRegistry
        {
            public TypeRegistry(Type sourceType, Type targetType = null)
            {
                if (sourceType == null) 
                    throw new ArgumentNullException("sourceType");

                SourceType = sourceType;
                TargetType = targetType ?? sourceType;
            }

            public Type SourceType { get; private set; }
            public Type TargetType { get; private set; }
            public bool IsMatch(Type type)
            {
                return SourceType == type;
            }
        }

        public void Register(Type serviceType)
        {
            _registrations.Add(new TypeRegistry (serviceType));
        }
    }

    public class SingletonResolve : IResolver
    {
        public Resolvable Resolve(Type type)
        {
            throw new NotImplementedException();
        }
    }

    public static class ResolverExtensions
    {
        public static T Resolve<T>(this IResolver resolver)
        {
            var resolvable = resolver.Resolve(typeof (T));
            if (resolvable == null)
                return default(T);
            return (T) resolvable.Build(resolver);
        }

        public static Resolver Register<T>(this Resolver resolver)
        {
            resolver.Register(typeof(T));
            return resolver;
        }
    }
}