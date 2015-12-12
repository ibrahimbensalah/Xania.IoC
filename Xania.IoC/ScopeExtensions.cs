using System;

namespace Xania.IoC
{
    public static class ScopeExtensions
    {
        public static T Resolve<T>(this IObjectContainer objectContainer)
            where T: class 
        {
            var instance = objectContainer.Resolve(typeof (T));
            if (instance != null && !(instance is T))
                throw new InvalidOperationException();

            return instance as T;
        }
    }
}