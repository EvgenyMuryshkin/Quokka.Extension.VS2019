using Microsoft.VisualStudio.Shell;
using Quokka.Extension.Interface;
using System;

namespace Quokka.Extension.VS2019
{
    public class ExtensionDeps
    {
        public readonly IExtensionLogger Logger;
        public readonly AsyncPackage ServiceProvider;
        public readonly IJoinableTaskFactory TaskFactory;
        public ExtensionDeps(IExtensionLogger logger, AsyncPackage serviceProvider, IJoinableTaskFactory taskFactory)
        {
            Logger = logger;
            ServiceProvider = serviceProvider;
            TaskFactory = taskFactory;
        }
    }
}
