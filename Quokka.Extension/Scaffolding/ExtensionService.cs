using Quokka.Extension.Interface;
using System;

namespace Quokka.Extension.Scaffolding
{
    public abstract class ExtensionService
    {
        private readonly ExtensionDeps _deps;
        protected IExtensionLogger _logger => _deps.Logger;
        protected IServiceProvider _serviceProvider => _deps.ServiceProvider;
        protected IJoinableTaskFactory _taskFactory => _deps.TaskFactory;
        protected IExtensionNotificationService _ens => _deps.ExtensionNotificationService;

        public ExtensionService(ExtensionDeps deps)
        {
            _deps = deps;
        }
    }
}
