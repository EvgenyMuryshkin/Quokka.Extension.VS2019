using Quokka.Extension.Services;
using System.Collections.Generic;

namespace Quokka.Extension.Interface
{
    public interface IExtensionsDiscoveryService
    {
        List<ExtensionMethodInfo> LoadFromDirectory(string directory);
    }
}
