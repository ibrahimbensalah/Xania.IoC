using System;
using System.Collections.Generic;

namespace Xania.IoC
{
    public interface IResolvable
    {
        object Build(IResolver resolver);
    }
}