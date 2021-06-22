using Quokka.Extension.Interop;
using System.Threading.Tasks;

namespace Quokka.Extension.Examples
{
    public class Program
    {
        static async Task<int> Main(string[] args)
        {
            return await ExtensionInteropMain.Main(args);
        }
    }
}
