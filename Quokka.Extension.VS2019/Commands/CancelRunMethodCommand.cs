using Quokka.Extension.Scaffolding;
using System.Threading.Tasks;

namespace Quokka.Extension.VS2019
{
    internal class CancelRunMethodCommand : AsyncExtensionCommand
    {
        public CancelRunMethodCommand(ExtensionDeps deps) 
            : base(deps, guidQuokkaExtensionVS2019PackageIds.cmdidCancelRunMethodCommand)
        {

        }

        protected override Task OnExecuteAsync()
        {
            //await QuokkaExtensionVS2019Package.Instance.RerunExtensionMethodAsync();
            WriteLine("Cancel run");
            return Task.CompletedTask;
        }
    }
}
