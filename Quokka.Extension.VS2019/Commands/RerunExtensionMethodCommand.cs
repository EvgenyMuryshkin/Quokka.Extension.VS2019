using Quokka.Extension.Interface;
using System.Threading.Tasks;

namespace Quokka.Extension.VS2019
{
    internal class RerunExtensionMethodCommand : AsyncExtensionCommand
    {
        private readonly IExtensionInvocationService _invocationService;
        public RerunExtensionMethodCommand(ExtensionDeps deps, IExtensionInvocationService invocationService) 
            : base(deps, guidQuokkaExtensionVS2019PackageIds.cmdidRerunExtensionMethodCommand)
        {
            _invocationService = invocationService;
        }

        protected override async Task OnExecuteAsync()
        {
            await _invocationService.RerunExtensionMethodAsync();
        }
    }
}
