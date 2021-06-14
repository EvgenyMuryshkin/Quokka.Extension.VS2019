using Quokka.Extension.Interface;
using System.Collections.Generic;

namespace Quokka.Extension.Services
{
    public class ExtensionsCacheService : IExtensionsCacheService
    {
        private readonly IExtensionsDiscoveryService _eds;
        public ExtensionsCacheService(IExtensionsDiscoveryService eds)
        {
            _eds = eds;
        }

        public List<ExtensionMethodInfo> Extensions { get; private set; } = new List<ExtensionMethodInfo>();

        public void Reload(string directory)
        {
            Extensions = _eds.LoadFromDirectory(directory);
        }
    }
}
