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

        public IEnumerable<IDependency> GetDependencies()
        {
            return _ctor.GetParameters().Select(p => new TypeDependency(p.ParameterType));
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
            var q =
                from ctor in implementationType.GetConstructors()
                where ctor.GetParameters().All(p => p.ParameterType.IsClass || p.ParameterType.IsInterface)
                orderby ctor.GetParameters().Length descending 
                select ctor;

            if (q.Any())
                return new TypeResolvable(implementationType, q.FirstOrDefault());

            return null;
        }
    }
}