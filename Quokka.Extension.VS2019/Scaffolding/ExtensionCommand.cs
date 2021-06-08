using System;

namespace Quokka.Extension.VS2019
{
    internal abstract class ExtensionCommand : ExtensionBaseCommand
    {
        protected ExtensionCommand(ExtensionPartDeps deps, int commandId) : base(deps, commandId)
        {
        }

        protected abstract void OnExecute();

        protected override void Execute(object sender, EventArgs e)
        {
            OnExecute();
        }
    }
}
