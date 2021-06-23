using Quokka.Extension.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quokka.Extension.Interface
{
    public interface IExtensionsDiscoveryService
    {
        Task<List<ExtensionMethodInfo>> LoadFromDirectoryAsync(string directory);
    }
}
