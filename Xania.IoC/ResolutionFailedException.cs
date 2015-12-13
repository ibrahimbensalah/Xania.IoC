using System;
using System.Collections.Generic;
using System.Linq;

namespace Xania.IoC
{
    public class ResolutionFailedException : Exception
    {
        private readonly Type _type;

        public ResolutionFailedException(Type type)
        {
            _type = type;
        }
    }
}