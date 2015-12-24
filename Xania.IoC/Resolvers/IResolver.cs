using System;
using System.Collections.Generic;

namespace Xania.IoC.Resolvers
{
    public interface IResolver
    {
        IEnumerable<IResolvable> ResolveAll(Type serviceType);
    }
}