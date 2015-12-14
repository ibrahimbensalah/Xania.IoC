using Xania.IoC.Resolvers;

namespace Xania.IoC
{
    public interface IResolvable
    {
        object Build(IResolver resolver);
    }
}