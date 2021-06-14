using Quokka.Extension.Interface;
using Quokka.Extension.Scaffolding;
using Quokka.Extension.Services;
using System.Threading.Tasks;

namespace Quokka.Extension.ViewModels
{
    public class ExtensionMethodInvocationCommandViewModel : AsyncCommandViewModel
    {
        private readonly IExtensionInvocationService _invocationService;
        private readonly ExtensionMethodInfo _invokeParams;

        public delegate ExtensionMethodInvocationCommandViewModel Factory(ExtensionMethodInfo invokeParams);

        public ExtensionMethodInvocationCommandViewModel(
            ExtensionDeps deps, 
            IExtensionInvocationService invocationService, 
            ExtensionMethodInfo invokeParams) : base(deps)
        {
            _invocationService = invocationService;
            _invokeParams = invokeParams;
        }

        protected override async Task ExecuteAsync(object parameter)
        {
            await _invocationService.InvokeExtensionMethodAsync(_invokeParams);
        }
    }
}
