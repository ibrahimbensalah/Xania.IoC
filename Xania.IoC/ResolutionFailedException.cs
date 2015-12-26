using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Xania.IoC
{
    public class ResolutionFailedException : Exception
    {
        public IList<Type> Types { get; private set; }

        public ResolutionFailedException(params Type[] types)
        {
            Types = new List<Type>(types);
        }

        public override string Message
        {
            get
            {
                var stringBuilder = new StringBuilder();
                var path = string.Join("\r\n -> ", this.Types.Select(c => c.FullName));
                return "Resolution failed for type: \r\n -> " +  path;
            }
        }
    }
}