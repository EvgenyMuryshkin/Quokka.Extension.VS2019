using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Quokka.Extension.Interface;
using Quokka.Extension.Scaffolding;
using Quokka.Extension.ViewModels;
using Quokka.Extension.VS2022.Scaffolding;
using System;
using System.IO;
using Task = System.Threading.Tasks.Task;

namespace Quokka.Extension.VS2022
{
    internal class ReloadCommand : AsyncExtensionCommand
    {
        private readonly IExtensionsCacheService _ecs;
        public ReloadCommand(
            ExtensionDeps deps, IExtensionsCacheService ecs)
            : base(deps, guidQuokkaExtensionVS2022PackageIds.cmdidReloadCommand)
        {
            _ecs = ecs;
        }

        protected override async Task OnExecuteAsync()
        {
            await _taskFactory.SwitchToMainThreadAsync();
            await _ecs.Reload(trace: true);
        }
    }
}
