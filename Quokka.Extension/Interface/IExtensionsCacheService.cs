using Quokka.Extension.Interop;
using Quokka.Extension.Services;
using System.Collections.Generic;

namespace Quokka.Extension.Interface
{
    public interface IExtensionsCacheService
    {
        List<ExtensionMethodInfo> Extensions { get; }
        void Reload(string directory);
        List<ExtensionMethodInfo> ExtensionsForIcon(ExtensionMethodIcon icon);
    }
}
