using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Xania.IoC.Resolvers;

namespace Xania.IoC
{
    public class TypeResolvable : IResolvable
    {
        private readonly ConstructorInfo _ctor;
        private readonly object[] _values;
        private readonly ParameterInfo[] _parameters;

        public TypeResolvable(Type serviceType, ConstructorInfo ctor, object[] values)
        {
            if (ctor == null)
                throw new ArgumentNullException("ctor");

            _parameters = ctor.GetParameters();

            if (values != null && values.Length != _parameters.Length)
                throw new ArgumentException("args length does not match parameters length");

            _ctor = ctor;
            _values = values;
            ServiceType = serviceType;
        }

        public IEnumerable<IDependency> GetDependencies()
        {
            if (_values != null)
            {
                for (var i = 0; i < this._values.Length; i++)
                {
                    if (this._values[i] == null)
                        yield return new TypeDependency(_parameters[i].ParameterType);
                }
            }
            else
            {
                foreach(var p in _parameters)
                    yield return new TypeDependency(p.ParameterType);
            }
        }

        public object Create(params object[] args)
        {
            if (args.Any(x => x == null))
                return null;

            if (_values == null || _values.Length == 0)
                return _ctor.Invoke(args);

            if (args == null || args.Length == 0)
                return _ctor.Invoke(_values);

            var values = (object[])_values.Clone();
            var enumer = args.GetEnumerator();

            for (var i = 0; i < values.Length; i++)
            {
                if (values[i] == null)
                {
                    if (!enumer.MoveNext())
                        throw new ArgumentException("supplied arguments does not match required dependencies");

                    values[i] = enumer.Current;
                }
            }
            if (enumer.MoveNext())
                throw new ArgumentException("supplied arguments does not match required dependencies");

            return _ctor.Invoke(values);
        }

        public Type ServiceType { get; private set; }

        public static TypeResolvable Create(Type implementationType, ConstructorArgs args)
        {
            var q =
                from c in implementationType.GetConstructors()
                where c.GetParameters().All(p => p.ParameterType.IsClass || p.ParameterType.IsInterface)
                orderby c.GetParameters().Length descending
                select c;

            if (!q.Any())
                return null;

            if (args == null)
            {
                var ctor = q.FirstOrDefault();
                return new TypeResolvable(implementationType, ctor, null);
            }
            else
            {
                var matches = q.Where(args.Matches).ToArray();
                if (!matches.Any())
                    return null;

                var ctor = matches.FirstOrDefault();
                return new TypeResolvable(implementationType, ctor, args.Values);
            }
        }

        public override int GetHashCode()
        {
            return ServiceType.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is TypeResolvable && Equals((TypeResolvable)obj);
        }

        public bool Equals(TypeResolvable typeResolvable)
        {
            return typeResolvable.ServiceType == ServiceType;
        }
    }
}