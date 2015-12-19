using System;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace Xania.IoC.Resolvers
{
    internal class ScopeDecoraptor : RealProxy, IDisposable
    {
        private readonly Type _type;
        private readonly Func<object> _factory;
        private readonly ScopeProvider _scopeProvider;

        public ScopeDecoraptor(Type type, Func<object> factory, ScopeProvider scopeProvider)
            : base(type)
        {
            _type = type;
            _factory = factory;
            _scopeProvider = scopeProvider;
        }

        public override IMessage Invoke(IMessage msg)
        {
            var instance = GetInstance();
            var methodCall = msg as IMethodCallMessage;

            if (instance == null)
                return new ReturnMessage(new NullReferenceException(), methodCall);

            try
            {
                Debug.Assert(methodCall != null, "methodCall != null");
                var methodInfo = methodCall.MethodBase;
                // Console.WriteLine("Precall " + methodInfo.Name);
                var result = methodInfo.Invoke(instance, methodCall.InArgs);
                // Console.WriteLine("Postcall " + methodInfo.Name);

                return new ReturnMessage(result, null, 0,
                    methodCall.LogicalCallContext, methodCall);
            }
            catch(Exception ex)
            {
                return new ReturnMessage(ex, methodCall);
            }
        }

        private object GetInstance()
        {
            return _scopeProvider.Get().Get(_type, _factory);
        }

        public void Dispose()
        {
            _scopeProvider.Dispose();
        }
    }
}