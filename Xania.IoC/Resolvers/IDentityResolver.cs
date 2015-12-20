using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xania.IoC.Resolvers
{
    public class IdentityResolver: IResolver
    {
        public ICollection<Func<Type, bool>> Predicates { get; private set; }

        public IdentityResolver()
        {
            Predicates = new List<Func<Type, bool>>();
        }

        public IdentityResolver Include(Func<Type, bool> predicate)
        {
            Predicates.Add(predicate);
            return this;
        }

        public IdentityResolver For<T>()
        {
            Include(t => typeof (T).IsAssignableFrom(t));
            return this;
        }

        public IResolvable Resolve(Type type)
        {
            return Predicates.Any(p => p(type)) ? TypeResolvable.Create(type) : null;
        }
    }
}
