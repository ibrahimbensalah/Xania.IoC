using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Xania.IoC
{
    public class ResolutionFailedException : Exception
    {
        public IList<Type> Types { get; private set; }

        public ResolutionFailedException(IDependency dependency, params Type[] types)
        {
            Dependency = dependency;
            Types = new List<Type>(types);
        }

        public IDependency Dependency { get; private set; }

        public override string Message
        {
            get
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat("Resolution failed for type: \r\n {0}", Dependency);
                foreach (var type in this.Types)
                    stringBuilder.AppendFormat("\r\n <- {0} ", type.FullName);
                return stringBuilder.ToString();
            }
        }
    }
}