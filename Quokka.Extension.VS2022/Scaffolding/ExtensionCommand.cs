using Quokka.Extension.Scaffolding;
using System;

namespace Quokka.Extension.VS2022
{
    internal abstract class ExtensionCommand : ExtensionBaseCommand
    {
        protected ExtensionCommand(ExtensionDeps deps, int commandId) : base(deps, commandId)
        {
        }

        protected abstract void OnExecute();

        protected override void Execute(object sender, EventArgs e)
        {
            OnExecute();
        }
    }
}
