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
        IEnumerable<Type> GetDependencies();
    }
}