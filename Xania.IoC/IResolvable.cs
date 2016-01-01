using System;
using System.Collections.Generic;
using Xania.IoC.Resolvers;

namespace Xania.IoC
{
    public interface IResolvable
    {
        Type ServiceType { get; }
        // object Build(IResolver resolver);

        object Create(params object[] args);
        IEnumerable<IDependency> GetDependencies();
    }

    public interface IDependency
    {
        object Build(IResolver resolver);
    }

    public class TypeDependency : IDependency
    {
        private readonly Type _type;

        public TypeDependency(Type type)
        {
            _type = type;
        }

        public virtual object Build(IResolver resolver)
        {
            var dep = resolver.Resolve(_type);
            if (dep != null)
                return dep.Build(resolver);
            return null;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TypeDependency);
        }

        public override int GetHashCode()
        {
            return _type.GetHashCode();
        }

        protected bool Equals(TypeDependency other)
        {
            if (other == null)
                return false;
            return _type == other._type;
        }

        public override string ToString()
        {
            return this._type.ToString();
        }
    }
}