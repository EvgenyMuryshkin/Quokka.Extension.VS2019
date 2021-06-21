using Quokka.Extension.Interface;
using Quokka.Extension.Interop;
using System.Collections.Generic;
using System.Linq;

namespace Quokka.Extension.Services
{
    public class ExtensionsCacheService : IExtensionsCacheService
    {
        private readonly IExtensionLogger _logger;
        private readonly IExtensionsDiscoveryService _eds;
        private readonly IExtensionNotificationService _ens;

        public ExtensionsCacheService(IExtensionLogger logger, IExtensionsDiscoveryService eds, IExtensionNotificationService ens)
        {
            _logger = logger;
            _eds = eds;
            _ens = ens;
        }
        public string Solution { get; set; }

        public List<ExtensionMethodInfo> Extensions { get; private set; } = new List<ExtensionMethodInfo>();
        Dictionary<string, List<ExtensionMethodInfo>> _mapIconToExtensions = new Dictionary<string, List<ExtensionMethodInfo>>();

        public List<ExtensionMethodInfo> ExtensionsForIcon(ExtensionMethodIcon icon)
        {
            _mapIconToExtensions.TryGetValue(icon.ToString(), out var result);
            return result ?? new List<ExtensionMethodInfo>();
        }

        public void Reload(string solution = null, bool trace = false)
        {
            Solution = solution ?? Solution;
            Extensions = _eds.LoadFromDirectory(Solution);
            _mapIconToExtensions = Extensions.GroupBy(e => e.Icon.ToString()).ToDictionary(k => k.Key, v => v.ToList());
            
            if (trace)
                Extensions.ForEach(invokeParams => _logger.WriteLine($"Found extension method: {invokeParams.Class}.{invokeParams.Method}"));

            _ens.RaiseSolutionChanged(Solution);
        }

        public void Close()
        {
            Solution = "";
            Extensions.Clear();
            _ens.RaiseSolutionChanged(Solution);
        }
    }
}
