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
        private readonly ExtensionInvocationService _invocationService;
        private readonly ExtensionMethodInvokeParams _invokeParams;
        public ExtensionMethodInvocationCommandViewModel(ExtensionInvocationService invocationService, ExtensionMethodInvokeParams invokeParams)
        {
            _invocationService = invocationService;
            _invokeParams = invokeParams;
        }

        protected override async Task ExecuteAsync(object parameter)
        {
            await _invocationService.InvokeExtensionMethodAsync(_invokeParams);
        }
    }
}
