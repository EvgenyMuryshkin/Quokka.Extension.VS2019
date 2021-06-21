using Quokka.Extension.Scaffolding;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Quokka.Extension.Interface
{
    public interface IJoinableTaskFactory
    {
        T Run<T>(Func<Task<T>> asyncMethod);
        Task RunAsync(Func<Task> asyncMethod);
        MainThreadAwaitableWrapper SwitchToMainThreadAsync(CancellationToken cancellationToken = default);
    }
}
