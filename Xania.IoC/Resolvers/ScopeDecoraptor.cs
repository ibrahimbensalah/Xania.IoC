using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace Xania.IoC.Resolvers
{
    internal class ScopeDecoraptor : RealProxy, IDisposable
    {
        private readonly Type _type;
        private readonly Func<object> _factory;
        private readonly IScopeProvider _scopeProvider;

        public ScopeDecoraptor(Type type, Func<object> factory, IScopeProvider scopeProvider)
            : base(type)
        {
            _type = type;
            _factory = factory;
            _scopeProvider = scopeProvider;
        }

        public override IMessage Invoke(IMessage msg)
        {
            var methodCall = msg as IMethodCallMessage;
            Debug.Assert(methodCall != null, "methodCall != null");
            try
            {
                if (methodCall.MethodBase.DeclaringType == typeof (IDisposable) &&
                    methodCall.MethodName == "Dispose")
                {
                    Dispose();
                    return new ReturnMessage(null, null, 0,
                        methodCall.LogicalCallContext, methodCall);
                }
                else
                {
                    return Invoke(methodCall);
                }
            }
            catch (Exception ex)
            {
                return new ReturnMessage(ex, methodCall);
            }
        }

        private IMessage Invoke(IMethodCallMessage methodCall)
        {
            var instance = GetInstance();
            if (instance == null)
                return new ReturnMessage(new NullReferenceException(), methodCall);

            var methodInfo = methodCall.MethodBase;
            // Console.WriteLine("Precall " + methodInfo.Name);
            var result = methodInfo.Invoke(instance, methodCall.InArgs);
            // Console.WriteLine("Postcall " + methodInfo.Name);

            return new ReturnMessage(result, null, 0, methodCall.LogicalCallContext, methodCall);
        }

        private object GetInstance()
        {
            return _scopeProvider.Get().Get(_type, _factory);
        }

        public void Dispose()
        {
            _scopeProvider.Get().Dispose(_type);
        }
    }
}