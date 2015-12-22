using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xania.IoC.Resolvers
{
    public class IdentityResolver: IResolver
    {
        public readonly ICollection<Type> BaseTypes;

        public IdentityResolver()
        {
            BaseTypes = new List<Type>();
        }

        public IdentityResolver For(Type baseType)
        {
            BaseTypes.Add(baseType);
            return this;
        }

        public IdentityResolver For<T>()
        {
            For(typeof (T));
            return this;
        }

        public IEnumerable<IResolvable> ResolveAll(Type type)
        {
            if (BaseTypes.Any(x => IsAssignableFrom(x, type)))
                yield return TypeResolvable.Create(type);
        }

        private static bool IsAssignableFrom(Type baseType, Type type)
        {
            if (!baseType.IsGenericTypeDefinition)
                return baseType.IsAssignableFrom(type);

            var stack = new Stack<Type>();
            stack.Push(type);
            foreach (var i in type.GetInterfaces())
                stack.Push(i);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                if (current.IsGenericType)
                {
                    var genericType = current.GetGenericTypeDefinition();
                    if (baseType == genericType)
                        return true;
                }

                if (current.BaseType != null)
                    stack.Push(current.BaseType);
            }

            return false;
        }
    }
}
