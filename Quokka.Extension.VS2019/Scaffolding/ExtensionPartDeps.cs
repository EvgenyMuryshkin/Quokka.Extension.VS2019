using Microsoft.VisualStudio.Shell;
using System;

namespace Quokka.Extension.VS2019
{
    internal class ExtensionPartDeps
    {
        public readonly IExtensionLogger Logger;
        public readonly AsyncPackage ServiceProvider;

        public ExtensionPartDeps(IExtensionLogger logger, AsyncPackage serviceProvider)
        {
            Logger = logger;
            ServiceProvider = serviceProvider;

        }
    }
}
