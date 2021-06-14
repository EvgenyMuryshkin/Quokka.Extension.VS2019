using Quokka.Extension.Services;

namespace Quokka.Extension.Interface
{
    public interface IInvocationCacheService
    {
        void Push(ExtensionMethodInfo info);
        ExtensionMethodInfo Peek();
        void Clear();
    }
}
