using System;

namespace Xania.IoC
{
    public interface IResolver
    {
        IResolvable Resolve(Type type);
    }
}