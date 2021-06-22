using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Quokka.Extension.Interface;
using Quokka.Extension.Scaffolding;
using Quokka.Extension.ViewModels;
using Quokka.Extension.VS2019.Scaffolding;
using System;
using System.IO;
using Task = System.Threading.Tasks.Task;

namespace Quokka.Extension.VS2019
{
    internal class ReloadCommand : AsyncExtensionCommand
    {
        private readonly IExtensionsCacheService _ecs;
        public ReloadCommand(
            ExtensionDeps deps, IExtensionsCacheService ecs)
            : base(deps, guidQuokkaExtensionVS2019PackageIds.cmdidReloadCommand)
        {
            _ecs = ecs;
        }

        protected override async Task OnExecuteAsync()
        {
            await TaskFactory.SwitchToMainThreadAsync();
            await _ecs.Reload(trace: true);
        }
    }
}
