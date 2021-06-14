using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Quokka.Extension.Interface;
using Quokka.Extension.Services;

namespace Quokka.Extension.Tests
{
    [TestClass]
    public class ExtensionsTreeViewModelBuilderTests
    {
        [TestMethod]
        public void FromFolder()
        {
            var logger = new Mock<IExtensionLogger>();
            var svc = new ExtensionsDiscoveryService(logger.Object);
            var extensions = svc.LoadFromDirectory(@"C:\code\Quokka.Extension.VS2019");
            Assert.AreEqual(2, extensions.Count);
        }
    }
}
