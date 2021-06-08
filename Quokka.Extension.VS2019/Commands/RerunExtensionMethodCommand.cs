using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Quokka.Extension.VS2019
{
    internal class RerunExtensionMethodCommand : AsyncExtensionCommand
    {
        private readonly ExtensionInvocationService _invocationService;
        public RerunExtensionMethodCommand(ExtensionPartDeps deps, ExtensionInvocationService invocationService) 
            : base(deps, guidQuokkaExtensionVS2019PackageIds.cmdidRerunExtensionMethodCommand)
        {
            _invocationService = invocationService;
        }

        protected override async Task OnExecuteAsync()
        {
            await _invocationService.RerunExtensionMethodAsync();
        }
    }
}
