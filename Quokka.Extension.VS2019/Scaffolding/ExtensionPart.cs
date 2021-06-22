using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Quokka.Extension.Interface;
using Quokka.Extension.Scaffolding;
using System;
using System.ComponentModel.Design;
using Task = System.Threading.Tasks.Task;

namespace Quokka.Extension.VS2019
{
    internal abstract class ExtensionPart : IExtensionPart
    {
        private ExtensionDeps _deps;

        protected IExtensionLogger _logger => _deps.Logger;
        protected IServiceProvider _serviceProvider => _deps.ServiceProvider;
        protected IJoinableTaskFactory TaskFactory => _deps.TaskFactory;
        protected IExtensionPackage Package => _deps.Package;

        protected IVsSolution Solution => _serviceProvider.GetService<IVsSolution, IVsSolution>();
        protected OleMenuCommandService CommandService => _serviceProvider.GetService<IMenuCommandService, OleMenuCommandService>();
        protected IVsOutputWindow OutputWindow => _serviceProvider.GetService<SVsOutputWindow, IVsOutputWindow>();
        protected IExtensionNotificationService ExtensionNotificationService => _deps.ExtensionNotificationService;

        public ExtensionPart(ExtensionDeps deps)
        {
            _deps = deps;
        }

        protected abstract Task OnInitializeAsync();

        public virtual async Task InitializeAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            await OnInitializeAsync();
        }

        protected void Write(string message) => _logger.Write(message);
        protected void WriteLine(string message) => _logger.WriteLine(message);
    }
}
