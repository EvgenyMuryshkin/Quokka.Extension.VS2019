using Quokka.Extension.Interface;
using Quokka.Extension.Scaffolding;
using System;
using System.ComponentModel.Design;
using System.Reactive.Concurrency;
using System.Threading.Tasks;

namespace Quokka.Extension.VS2019
{
    internal class CancelRunMethodCommand : AsyncExtensionCommand
    {
        private readonly IExtensionInvocationService _invocationService;

        public CancelRunMethodCommand(ExtensionDeps deps, IExtensionInvocationService invocationService) 
            : base(deps, guidQuokkaExtensionVS2019PackageIds.cmdidCancelRunMethodCommand)
        {
            _invocationService = invocationService;

            _invocationService.InvocationEvent += OnInvocationEvent;
        }

        void OnInvocationEvent(object sender, EventArgs arg)
        {
            GetCommand.Enabled = _invocationService.IsRunning;
        }

        protected override void OnCommandCreated(MenuCommand command)
        {
            command.Enabled = false;
        }

        protected override Task OnExecuteAsync()
        {
            var asyncTask = new Task(() => _invocationService.CancelRun());
            asyncTask.Start(TaskScheduler.Default);
            return Task.CompletedTask;
        }
    }
}
