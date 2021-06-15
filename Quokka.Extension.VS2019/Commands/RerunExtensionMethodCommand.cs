using Quokka.Extension.Interface;
using Quokka.Extension.Scaffolding;
using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;

namespace Quokka.Extension.VS2019
{
    internal class RerunExtensionMethodCommand : AsyncExtensionCommand
    {
        private readonly IExtensionInvocationService _invocationService;
        private readonly IInvocationCacheService _invocationCacheService;
        public RerunExtensionMethodCommand(
            ExtensionDeps deps, 
            IExtensionInvocationService invocationService,
            IInvocationCacheService invocationCacheService) 
            : base(deps, guidQuokkaExtensionVS2019PackageIds.cmdidRerunExtensionMethodCommand)
        {
            _invocationService = invocationService;
            _invocationCacheService = invocationCacheService;

            _invocationService.InvocationEvent += OnInvocationEvent;
        }

        void OnInvocationEvent(object sender, EventArgs arg)
        {
            GetCommand.Enabled = !_invocationService.IsRunning && _invocationCacheService.Peek() != null;
        }

        protected override void OnCommandCreated(MenuCommand command)
        {
            command.Enabled = false;
        }

        protected override async Task OnExecuteAsync()
        {
            await _invocationService.RerunExtensionMethodAsync();
        }
    }
}
