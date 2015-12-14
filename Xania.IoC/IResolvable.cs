using System;
using Xania.IoC.Resolvers;

namespace Xania.IoC
{
    public interface IResolvable
    {
        object Build(IResolver resolver);

        Type ServiceType { get; }
    }
}