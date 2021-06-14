using System;
using System.Threading;
using System.Threading.Tasks;

namespace Quokka.Extension.Interface
{
    public interface IJoinableTaskFactory
    {
        Task Run(Func<Task> asyncMethod);
        Task SwitchToMainThreadAsync(CancellationToken cancellationToken = default);
    }
}
