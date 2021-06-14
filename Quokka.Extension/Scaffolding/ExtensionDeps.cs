using Quokka.Extension.Interface;
using System;

namespace Quokka.Extension.Scaffolding
{
    public class ExtensionDeps
    {
        public readonly IExtensionLogger Logger;
        public readonly IServiceProvider ServiceProvider;
        public readonly IJoinableTaskFactory TaskFactory;
        public ExtensionDeps(IExtensionLogger logger, IServiceProvider serviceProvider, IJoinableTaskFactory taskFactory)
        {
            Logger = logger;
            ServiceProvider = serviceProvider;
            TaskFactory = taskFactory;
        }
    }
}
