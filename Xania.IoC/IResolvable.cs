using System;
using System.Collections.Generic;

namespace Xania.IoC
{
    public interface IResolvable
    {
        // object Create(params object[] args);

        object Build(IResolver resolver);

        // IEnumerable<Type> GetDependencies();
    }
}