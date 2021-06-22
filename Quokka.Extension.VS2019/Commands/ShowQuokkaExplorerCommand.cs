using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Quokka.Extension.Scaffolding;
using Quokka.Extension.ViewModels;
using System;
using System.IO;
using Task = System.Threading.Tasks.Task;

namespace Quokka.Extension.VS2019
{
    internal class ShowQuokkaExplorerCommand : AsyncExtensionCommand
    {
        public ShowQuokkaExplorerCommand(
            ExtensionDeps deps)
            : base(deps, guidQuokkaExtensionVS2019PackageIds.ShowQuokkaExplorerCommandId)
        {
        }

        protected override async Task OnExecuteAsync()
        {
            await TaskFactory.RunAsync(async delegate
            {
                await Package.ShowToolWindowAsync(typeof(QuokkaExplorer), 0, true);
            });
        }
    }
}
