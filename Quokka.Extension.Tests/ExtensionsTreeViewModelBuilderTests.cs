using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Quokka.Extension.Interface;
using Quokka.Extension.Services;
using System.Threading.Tasks;

namespace Quokka.Extension.Tests
{
    [TestClass]
    public class ExtensionsTreeViewModelBuilderTests
    {
        [TestMethod]
        public async Task FromFolder()
        {
            var logger = new Mock<IExtensionLogger>();
            var svc = new ExtensionsDiscoveryService();
            var extensions = await svc.LoadFromDirectoryAsync(@"C:\code\Quokka.Extension.VS2019");
            Assert.AreEqual(2, extensions.Count);
        }
    }
}
