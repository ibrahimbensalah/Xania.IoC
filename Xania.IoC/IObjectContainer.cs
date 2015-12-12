using System;

namespace Xania.IoC
{
    public interface IObjectContainer
    {
        object Resolve(Type serviceType);
    }
}