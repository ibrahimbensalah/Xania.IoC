using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Xania.IoC
{
    public class TypeResolvable: IResolvable
    {
        private readonly ConstructorInfo _ctor;

        public TypeResolvable(Type serviceType, ConstructorInfo ctor)
        {
            if (ctor == null) 
                throw new ArgumentNullException("ctor");

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

        public Type ServiceType { get; private set; }

        public static TypeResolvable Create(Type implementationType)
        {
            var ctor = implementationType
                .GetConstructors()
                .OrderByDescending(e => e.GetParameters().Length)
                .FirstOrDefault();

            if (ctor == null)
                return null;

            return new TypeResolvable(implementationType, ctor);
        }
    }
}