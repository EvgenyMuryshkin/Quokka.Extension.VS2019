using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Diagnostics;
using System.IO;
using Task = System.Threading.Tasks.Task;

namespace Quokka.Extension.VS2019
{
    class ExtensionMethodInvocationCommandViewModel : AsyncCommandViewModel
    {
        private readonly QuokkaExtensionVS2019Package _package;
        private readonly ExtensionMethodInvokeParams _invokeParams;
        public ExtensionMethodInvocationCommandViewModel(QuokkaExtensionVS2019Package package, ExtensionMethodInvokeParams invokeParams)
        {
            _package = package;
            _invokeParams = invokeParams;
        }

        protected override async Task ExecuteAsync(object parameter)
        {
            await _package.InvokeExtensionMethodAsync(_invokeParams);
        }
    }
}
