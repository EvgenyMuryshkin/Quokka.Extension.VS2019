using System.Threading.Tasks;

namespace Quokka.Extension.Interface
{
    public interface IExtensionLogger
    {
        Task Activate();
        void Write(string message);
        void WriteLine(string message);
    }
}
