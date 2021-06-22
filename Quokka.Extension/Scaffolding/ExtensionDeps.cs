using Quokka.Extension.Interface;
using System;

namespace Quokka.Extension.Scaffolding
{
    public class ExtensionDeps
    {
        public readonly IExtensionLogger Logger;
        public readonly IServiceProvider ServiceProvider;
        public readonly IJoinableTaskFactory TaskFactory;
        public readonly IExtensionPackage Package;
        public readonly IExtensionNotificationService ExtensionNotificationService;

        public ExtensionDeps(
            IExtensionLogger logger, 
            IServiceProvider serviceProvider, 
            IJoinableTaskFactory taskFactory, 
            IExtensionPackage package,
            IExtensionNotificationService extensionNotificationService
            )
        {
            Logger = logger;
            ServiceProvider = serviceProvider;
            TaskFactory = taskFactory;
            Package = package;
            ExtensionNotificationService = extensionNotificationService;
        }
    }
}
