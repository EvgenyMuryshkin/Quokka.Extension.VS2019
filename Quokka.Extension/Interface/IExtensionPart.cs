using System.Threading.Tasks;

namespace Quokka.Extension.Interface
{
    public interface IExtensionPart
    {
        Task InitializeAsync();
    }
}
