using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Quokka.Extension.Tests
{
    [TestClass]
    public class ExtensionsTreeViewModelBuilderTests
    {
        [TestMethod]
        public void FromFolder()
        {
            var svc = new ExtensionsDiscoveryService();
            svc.Reload(@"C:\code\Quokka.Extension.VS2019");
            Assert.AreEqual(2, svc.Extensions.Count);
        }
    }
}
