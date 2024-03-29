﻿using Quokka.Extension.Interface;
using Quokka.Extension.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Quokka.Extension.Services
{
    public class ExtensionsCacheService : IExtensionsCacheService
    {
        private readonly IExtensionLogger _logger;
        private readonly IExtensionsDiscoveryService _eds;
        private readonly IExtensionNotificationService _ens;
        private readonly IExtensionPackage _extensionPackage;
        private readonly IInvocationCacheService _ics;

        public ExtensionsCacheService(
            IExtensionLogger logger, 
            IExtensionsDiscoveryService eds, 
            IExtensionNotificationService ens,
            IExtensionPackage extensionPackage,
            IInvocationCacheService ics)
        {
            _logger = logger;
            _eds = eds;
            _ens = ens;
            _extensionPackage = extensionPackage;
            _ics = ics;

            _ens.OnSolutionChanged += OnSolutionChanged;
            _ens.OnSolutionClosed += OnSolutionChanged;
        }

        private void OnSolutionChanged(object sender, EventArgs e)
        {
            Task.Run(() => Reload(trace: true));
        }

        public string Solution { get; set; }
        public bool HasExtensions => _mapIconToExtensions.Any();

        public List<ExtensionMethodInfo> Extensions { get; private set; } = new List<ExtensionMethodInfo>();
        Dictionary<string, List<ExtensionMethodInfo>> _mapIconToExtensions = new Dictionary<string, List<ExtensionMethodInfo>>();

        public List<ExtensionMethodInfo> ExtensionsForIcon(ExtensionMethodIcon icon)
        {
            _mapIconToExtensions.TryGetValue(icon.ToString(), out var result);
            return result ?? new List<ExtensionMethodInfo>();
        }

        public async Task Reload(string solution = null, bool trace = false)
        {
            Solution = solution ?? await _extensionPackage.SolutionPathAsync();
            if (Solution == null)
            {
                Close();
            }
            else
            {
                var sw = new Stopwatch();
                sw.Start();

                Extensions = await _eds.LoadFromDirectoryAsync(Solution);
                _mapIconToExtensions = Extensions.GroupBy(e => e.Icon.ToString()).ToDictionary(k => k.Key, v => v.ToList());

                if (trace)
                    Extensions.ForEach(invokeParams => _logger.WriteLine($"Found extension method: {invokeParams.Class}.{invokeParams.Method}"));

                _logger.WriteLine($"Extensions discovered in {sw.ElapsedMilliseconds} ms");
                _ens.RaiseExtensionsReloaded();
            }
        }

        public void Close()
        {
            Solution = "";
            Extensions.Clear();
            _mapIconToExtensions.Clear();
            _ics.Clear();
            _ens.RaiseExtensionsReloaded();
        }
    }
}
