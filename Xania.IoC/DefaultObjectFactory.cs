using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xania.IoC
{
    public class DefaultObjectFactory : IObjectFactory
    {
        private readonly IObjectContainer _objectContainer;

        public DefaultObjectFactory(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        public object Create(Type type)
        {
            return new ObjectBuilder(type, _objectContainer).Build();
        }


        private class ObjectBuilder
        {
            private readonly Type _type;
            private readonly IObjectContainer _objectContainer;

            public ObjectBuilder(Type type, IObjectContainer objectContainer)
            {
                _type = type;
                _objectContainer = objectContainer;
            }

            public object Build()
            {
                var type = _type;
                var ctor = type.GetConstructors().Single();
                var args = ctor.GetParameters()
                    .Select(p => new
                    {
                        Type = p.ParameterType,
                        Value = _objectContainer.Resolve(p.ParameterType),
                    }).ToArray();

                if (args.Any(e => e.Value == null))
                    throw new ResolutionFailedException(null);

                return ctor.Invoke(args.Select(e => e.Value).ToArray());

            }
        }
    }
}