using System;

namespace Xania.IoC.Resolvers
{
    public interface IResolver
    {
        IResolvable Resolve(Type type);
    }
}