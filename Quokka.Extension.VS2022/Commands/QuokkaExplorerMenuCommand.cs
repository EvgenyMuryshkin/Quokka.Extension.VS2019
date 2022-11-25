using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Quokka.Extension.Scaffolding;
using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Quokka.Extension.VS2022
{
    internal class QuokkaExplorerMenuCommand : AsyncExtensionCommand
    {
        public QuokkaExplorerMenuCommand(
            ExtensionDeps deps)
            : base(deps, guidQuokkaExtensionVS2022PackageIds.QuokkaExplorerMenuCommandId)
        {
        }

        protected override async Task OnExecuteAsync()
        {
            await _taskFactory.RunAsync(async delegate
            {
                await _package.ShowToolWindowAsync(typeof(QuokkaExplorer), 0, true);
            });
        }
    }
}
