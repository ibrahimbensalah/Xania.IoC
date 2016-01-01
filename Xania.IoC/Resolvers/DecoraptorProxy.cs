using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace Xania.IoC.Resolvers
{

    //public class DecoraptorResolver : ResolverCollection
    //{
    //    public override IEnumerable<IResolvable> ResolveAll(Type serviceType)
    //    {
    //        return
    //            from resolvable in base.ResolveAll(serviceType)
    //            select new DecoraptorResolvable(resolvable, new DictionaryBackingStore(() => new Dictionary<Type, object>(), type));
    //    }
    //}

    public class DecoraptorResolvable : IResolvable
    {
        private readonly IResolvable _resolvable;

        public DecoraptorResolvable(IResolvable resolvable)
        {
            _resolvable = resolvable;
        }

        public Type ServiceType
        {
            get { return _resolvable.ServiceType; }
        }

        public object Create(params object[] args)
        {
            //return new DecoraptorProxy(_type,
            //    new DictionaryBackingStore(() => _scopeProvider.Get(), t => _resolvable.Create(args))).GetTransparentProxy();
            throw new NotImplementedException();
        }

        public IEnumerable<IDependency> GetDependencies()
        {
            throw new NotImplementedException();
        }
    }

    internal interface IBackingStore
    {
        object Get(Type type);
        void Release(Type type);
    }

    internal class DictionaryBackingStore : IBackingStore
    {
        private readonly Func<IDictionary<Type, object>> _getDictionary;
        private readonly Func<Type, object> _factory;

        public DictionaryBackingStore(Func<IDictionary<Type, object>> getDictionary, Func<Type, object> factory)
        {
            _getDictionary = getDictionary;
            _factory = factory;
        }

        private IDictionary<Type, object> GetStore()
        {
            return _getDictionary();
        }

        public object Get(Type type)
        {
            return GetStore().Get(type, _factory);
        }

        public void Release(Type type)
        {
            _getDictionary().Dispose(type);
        }
    }

    internal class DecoraptorProxy : RealProxy
    {
        private readonly Type _type;
        private readonly IBackingStore _backingStore;

        public DecoraptorProxy(Type type, IBackingStore backingStore)
            : base(type)
        {
            _type = type;
            _backingStore = backingStore;
        }

        public override IMessage Invoke(IMessage msg)
        {
            var methodCall = msg as IMethodCallMessage;
            Debug.Assert(methodCall != null, "methodCall != null");
            try
            {
                if (methodCall.MethodBase.DeclaringType == typeof(IDisposable) &&
                    methodCall.MethodName == "Dispose")
                {
                    _backingStore.Release(_type);
                    return new ReturnMessage(null, null, 0,
                        methodCall.LogicalCallContext, methodCall);
                }
                else
                {
                    var instance = _backingStore.Get(_type);
                    if (instance == null)
                        return new ReturnMessage(new NullReferenceException(), methodCall);
                    return Invoke(instance, methodCall);
                }
            }
            catch (Exception ex)
            {
                return new ReturnMessage(ex, methodCall);
            }
        }

        private IMessage Invoke(object instance, IMethodCallMessage methodCall)
        {
            var methodInfo = methodCall.MethodBase;
            // Console.WriteLine("Precall " + methodInfo.Name);
            var result = methodInfo.Invoke(instance, methodCall.InArgs);
            // Console.WriteLine("Postcall " + methodInfo.Name);

            return new ReturnMessage(result, null, 0, methodCall.LogicalCallContext, methodCall);
        }
    }
}