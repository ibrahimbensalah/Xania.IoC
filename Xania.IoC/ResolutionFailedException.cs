using System;

namespace Xania.IoC
{
    public class ResolutionFailedException : Exception
    {
        public Type ServiceType { get; private set; }

        public ResolutionFailedException(Type type)
            : base("Resolution failed of service type: '" + type.FullName + "'")
        {
            ServiceType = type;
        }
    }
}