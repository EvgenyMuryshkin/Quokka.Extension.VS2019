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
    internal class CancelRunMethodCommand : AsyncExtensionCommand
    {
        public CancelRunMethodCommand(ExtensionPartDeps deps) : base(deps, guidQuokkaExtensionVS2019PackageIds.cmdidCancelRunMethodCommand)
        {

        }

        protected override Task OnExecuteAsync()
        {
            //await QuokkaExtensionVS2019Package.Instance.RerunExtensionMethodAsync();
            WriteLine("Cancel run");
            return Task.CompletedTask;
        }
    }
}
