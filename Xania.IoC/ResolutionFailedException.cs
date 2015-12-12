using System;
using System.Collections.Generic;
using System.Linq;

namespace Xania.IoC
{
    public class ResolutionFailedException : Exception
    {
        private readonly Resolvable _resolvable;

        public ResolutionFailedException(Resolvable resolvable)
        {
            _resolvable = resolvable;
        }
    }
}