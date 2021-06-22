using System;
using System.Threading.Tasks;

namespace Quokka.Extension.Interface
{
    public interface IExceptionHandler
    {
        Task OnException(string title, Exception ex);
    }
}
