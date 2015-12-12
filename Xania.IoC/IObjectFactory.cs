using System;

namespace Xania.IoC
{
    public interface IObjectFactory
    {
        object Create(Type serviceType);
    }
}