using System.Threading.Tasks;

namespace Quokka.Extension.VS2019
{
    internal interface IExtensionPart
    {
        Task InitializeAsync();
    }
}
