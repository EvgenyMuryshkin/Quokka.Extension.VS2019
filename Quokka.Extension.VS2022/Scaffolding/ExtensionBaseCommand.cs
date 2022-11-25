using Quokka.Extension.Scaffolding;
using System;
using System.ComponentModel.Design;
using Task = System.Threading.Tasks.Task;

namespace Quokka.Extension.VS2022
{
    internal abstract class ExtensionBaseCommand : ExtensionPart
    {
        protected readonly int _commandId;
        protected CommandID MenuCommandId => new CommandID(guidQuokkaExtensionVS2022PackageIds.guidQuokkaExtensionVS2022PackageCmdSet, _commandId);

        protected ExtensionBaseCommand(ExtensionDeps deps, int commandId) : base(deps)
        {
            _commandId = commandId;
        }

        protected MenuCommand GetCommand => CommandService.FindCommand(MenuCommandId);

        protected virtual void OnCommandCreated(MenuCommand command)
        {

        }

        protected override Task OnInitializeAsync()
        {
            var menuItem = new MenuCommand(Execute, MenuCommandId);
            OnCommandCreated(menuItem);
            CommandService.AddCommand(menuItem);
            return Task.CompletedTask;
        }

        protected abstract void Execute(object sender, EventArgs e);
    }
}
