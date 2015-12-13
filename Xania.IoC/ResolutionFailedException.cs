using System;
using System.Collections.Generic;
using System.Linq;

namespace Xania.IoC
{
    public class ResolutionFailedException : Exception
    {
        private readonly Type _type;
        private readonly TypeResolvable _typeResolvable;

        public ResolutionFailedException(TypeResolvable typeResolvable)
        {
            _typeResolvable = typeResolvable;
        }
        public ResolutionFailedException(Type type)
        {
            _type = type;
        }
    }
}