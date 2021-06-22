using Quokka.Extension.Interop;
using Quokka.Extension.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quokka.Extension.Interface
{
    public interface IExtensionsCacheService
    {
        string Solution { get; }
        bool HasExtensions { get; }
        List<ExtensionMethodInfo> Extensions { get; }
        Task Reload(string solution = null, bool trace = false);
        void Close();
        List<ExtensionMethodInfo> ExtensionsForIcon(ExtensionMethodIcon icon);
    }
}
