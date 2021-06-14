using System;
using System.ComponentModel.Design;
using Task = System.Threading.Tasks.Task;

namespace Quokka.Extension.VS2019
{
    internal abstract class ExtensionBaseCommand : ExtensionPart
    {
        protected readonly int _commandId;
        protected CommandID MenuCommandId => new CommandID(guidQuokkaExtensionVS2019PackageIds.guidQuokkaExtensionVS2019PackageCmdSet, _commandId);

        protected ExtensionBaseCommand(ExtensionDeps deps, int commandId) : base(deps)
        {
            _commandId = commandId;
        }

        protected override Task OnInitializeAsync()
        {
            var menuItem = new MenuCommand(Execute, MenuCommandId);
            CommandService.AddCommand(menuItem);
            return Task.CompletedTask;
        }

        protected abstract void Execute(object sender, EventArgs e);
    }
}
