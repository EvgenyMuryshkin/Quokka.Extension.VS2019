using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quokka.Extension.Interop;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Quokka.Extension.Tests
{
    [TestClass]
    public class IconsGenerator
    {
        IEnumerable<XElement> Recursive(XElement root)
        {
            return new[] { root }.Concat(root.Elements().SelectMany(e => Recursive(e)));
        }

        [TestMethod]
        public async Task Download()
        {
            var url = "https://react-icons.github.io";
            var client = new HttpClient();
            var content = await client.GetStringAsync($"{url}/react-icons");
            var x = XDocument.Parse(content);
            var flat = Recursive(x.Root);
            var menu = flat.Where(e => e.Name.LocalName == "ul").Single();
            var flatMenu = Recursive(menu);
            var flatA = flatMenu.Where(e => e.Name.LocalName == "a" && e.Value != "Home");
            var icons = flatA.ToDictionary(a => a.Value, a => a.Attribute("href").Value);

            foreach (var collection in icons)
            {
                var page = await client.GetStringAsync($"{url}{collection.Value}");

            }
        }
    }

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
