using Quokka.Extension.Interop;

namespace Quokka.Extension.Tests
{
    [ExtensionClass]
    public class TestExtensionClass
    {
        [ExtensionMethod]
        public static void VoidMethod()
        {
        }

        [ExtensionMethod]
        public static int IntMethod()
        {
            return 0;
        }
    }
}
