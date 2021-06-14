using Quokka.Extension.Services;
using System.Threading.Tasks;

namespace Quokka.Extension.Interface
{
    public interface IExtensionInvocationService
    {
        Task RerunExtensionMethodAsync();
        Task InvokeExtensionMethodAsync(ExtensionMethodInfo _invokeParams, bool pushToMRU = true);
    }
}
