using Quokka.Extension.Scaffolding;
using System;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Quokka.Extension.VS2019
{
    internal abstract class AsyncExtensionCommand : ExtensionBaseCommand
    {
        protected AsyncExtensionCommand(ExtensionDeps deps, int commandId) : base(deps, commandId)
        {
        }

        protected abstract Task OnExecuteAsync();

        protected override void Execute(object sender, EventArgs e)
        {
            var asyncTask = new Task(() => OnExecuteAsync());
            asyncTask.Start(TaskScheduler.Default);
        }
    }
}
