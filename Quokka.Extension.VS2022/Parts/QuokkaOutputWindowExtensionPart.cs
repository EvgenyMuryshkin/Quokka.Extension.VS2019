using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Quokka.Extension.Interface;
using System;
using Task = System.Threading.Tasks.Task;


namespace Quokka.Extension.VS2022
{
    internal class QuokkaOutputWindowExtensionPart : IExtensionLogger, IExtensionPart
    {
        readonly IServiceProvider _serviceProvider;
        IVsOutputWindow OutputWindow => _serviceProvider.GetService<SVsOutputWindow, IVsOutputWindow>();

        IVsOutputWindowPane QuokkaPane { get; set; }

        public QuokkaOutputWindowExtensionPart(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task InitializeAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var windowId = guidQuokkaExtensionVS2022PackageIds.QuokkaOutputWindowId;
            OutputWindow.GetPane(ref windowId, out var quokkaPane);
            if (quokkaPane == null)
            {
                OutputWindow.CreatePane(ref windowId, "Quokka", 1, 1);
                OutputWindow.GetPane(ref windowId, out quokkaPane);
                quokkaPane.Activate();
            }
            QuokkaPane = quokkaPane;
        }

        public async Task Activate()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            QuokkaPane?.Activate();
        }

        public void Write(string message) => QuokkaPane?.OutputStringThreadSafe(message);
        public void WriteLine(string message) => Write($"{message}{Environment.NewLine}");
    }
}
