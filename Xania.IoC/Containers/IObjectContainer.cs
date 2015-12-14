using System;

namespace Xania.IoC.Containers
{
    public interface IObjectContainer
    {
        object Resolve(Type serviceType);
    }
}