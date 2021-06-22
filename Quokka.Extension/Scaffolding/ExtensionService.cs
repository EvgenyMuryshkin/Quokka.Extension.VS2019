using Quokka.Extension.Interface;
using System;

namespace Quokka.Extension.Scaffolding
{
    public abstract class ExtensionService
    {
        private readonly ExtensionDeps _deps;
        protected IExtensionLogger Logger => _deps.Logger;
        protected IServiceProvider ServiceProvider => _deps.ServiceProvider;
        protected IJoinableTaskFactory TaskFactory => _deps.TaskFactory;
        protected IExtensionNotificationService ExtensionNotificationService => _deps.ExtensionNotificationService;

        public ExtensionService(ExtensionDeps deps)
        {
            _deps = deps;
        }
    }
}
