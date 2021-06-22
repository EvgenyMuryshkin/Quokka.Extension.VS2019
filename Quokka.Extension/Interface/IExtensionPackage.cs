using System;
using System.Threading;
using System.Threading.Tasks;

namespace Quokka.Extension.Interface
{
    public interface IExtensionPackage
    {
        Task ShowToolWindowAsync(Type toolWindowType, int id, bool create);
        CancellationToken DisposalToken { get; }
        Task<string> SolutionPathAsync();
    }
}
