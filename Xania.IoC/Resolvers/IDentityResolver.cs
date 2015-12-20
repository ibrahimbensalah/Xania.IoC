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

        public void Include(Func<Type, bool> predicate)
        {
            Predicates.Add(predicate);
        }

        public IResolvable Resolve(Type type)
        {
            return Predicates.Any(p => p(type)) ? TypeResolvable.Create(type) : null;
        }
    }
}
