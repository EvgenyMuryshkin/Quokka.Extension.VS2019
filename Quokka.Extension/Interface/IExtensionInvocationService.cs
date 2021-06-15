using Quokka.Extension.Services;
using System;
using System.Threading.Tasks;

namespace Quokka.Extension.Interface
{
    public interface IExtensionInvocationService
    {
        bool IsRunning { get; }
        Task RerunExtensionMethodAsync();
        Task InvokeExtensionMethodAsync(ExtensionMethodInfo _invokeParams, bool pushToMRU = true);
        void CancelRun();
        event EventHandler InvocationEvent;
    }
}
