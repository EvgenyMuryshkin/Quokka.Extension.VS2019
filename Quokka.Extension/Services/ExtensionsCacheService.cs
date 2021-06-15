using Quokka.Extension.Interface;
using Quokka.Extension.Interop;
using System.Collections.Generic;
using System.Linq;

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
        Dictionary<string, List<ExtensionMethodInfo>> _mapIconToExtensions = new Dictionary<string, List<ExtensionMethodInfo>>();

        public List<ExtensionMethodInfo> ExtensionsForIcon(ExtensionMethodIcon icon)
        {
            _mapIconToExtensions.TryGetValue(icon.ToString(), out var result);
            return result ?? new List<ExtensionMethodInfo>();
        }

        public void Reload(string directory)
        {
            Extensions = _eds.LoadFromDirectory(directory);
            _mapIconToExtensions = Extensions.GroupBy(e => e.Icon.ToString()).ToDictionary(k => k.Key, v => v.ToList());
        }
    }
}
