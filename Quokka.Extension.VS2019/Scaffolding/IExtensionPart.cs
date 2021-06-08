using Task = System.Threading.Tasks.Task;

namespace Quokka.Extension.VS2019
{
    internal interface IExtensionPart
    {
        Task InitializeAsync();
    }
}
