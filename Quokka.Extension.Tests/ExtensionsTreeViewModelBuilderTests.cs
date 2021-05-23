using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Quokka.Extension.Tests
{
    [TestClass]
    public class ExtensionsTreeViewModelBuilderTests
    {
        [TestMethod]
        public void FromFolder()
        {
            var viewModel = new ExtensionsTreeViewModel() { SolutionPath = @"C:\code\Quokka.Extension.VS2019" };
            ExtensionsTreeViewModelBuilder.PopulateViewModel(viewModel, null);

            Assert.AreEqual(1, viewModel.Projects.Count, "projects");
            Assert.AreEqual(1, viewModel.Projects[0].Classes.Count, "classes");
            Assert.AreEqual(2, viewModel.Projects[0].Classes[0].Methods.Count, "methods");
        }
    }
}
