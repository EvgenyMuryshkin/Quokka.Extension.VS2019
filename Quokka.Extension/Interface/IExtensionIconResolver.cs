using Quokka.Extension.Interop;

namespace Quokka.Extension.Interface
{
    public interface IExtensionIconResolver
    {
        object Resolve(ExtensionMethodIcon icon);
    }
}
