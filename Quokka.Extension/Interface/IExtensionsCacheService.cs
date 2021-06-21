using Quokka.Extension.Interop;
using Quokka.Extension.Services;
using System.Collections.Generic;

namespace Quokka.Extension.Interface
{
    public interface IExtensionsCacheService
    {
        string Solution { get; }
        List<ExtensionMethodInfo> Extensions { get; }
        void Reload(string solution = null, bool trace = false);
        void Close();
        List<ExtensionMethodInfo> ExtensionsForIcon(ExtensionMethodIcon icon);
    }
}
