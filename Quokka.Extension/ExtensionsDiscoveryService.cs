using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Quokka.Extension.Interop;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Quokka.Extension
{
    public class ExtensionsDiscoveryService
    {
        public List<ExtensionMethodInfo> Extensions = new List<ExtensionMethodInfo>();

        public void Reload(string solutionPath)
        {
            Extensions.Clear();

            if (File.Exists(solutionPath))
                solutionPath = Path.GetDirectoryName(solutionPath);

            if (!Directory.Exists(solutionPath))
                return;

            var projFiles = Directory.EnumerateFiles(solutionPath, "*.csproj", SearchOption.AllDirectories);

            foreach (var proj in projFiles)
            {
                var xProject = XDocument.Parse(File.ReadAllText(proj));
                var refs = xProject.Root
                    .Elements("ItemGroup")
                    .SelectMany(g => g.Elements("PackageReference"))
                    .Where(p => p.Attribute("Include")?.Value == "Quokka.Extension.Interop");

                if (refs.Any())
                {
                    var sources = Directory.EnumerateFiles(Path.GetDirectoryName(proj), "*.cs", SearchOption.AllDirectories);

                    foreach (var source in sources)
                    {
                        var sourceCode = File.ReadAllText(source);
                        SyntaxTree tree = CSharpSyntaxTree.ParseText(sourceCode);
                        var classDeclarations = tree
                            .GetRoot()
                            .DescendantNodes(n => true)
                            .OfType<ClassDeclarationSyntax>()
                            .Where(c => c.AttributeLists.Any(al => al.Attributes.Any(a => a.Name.ToString() == "ExtensionClass")));

                        foreach (var classDeclaration in classDeclarations)
                        {
                            var className = classDeclaration.Identifier.ToString();

                            var methodDeclarations = classDeclaration
                                .DescendantNodes(n => true)
                                .OfType<MethodDeclarationSyntax>()
                                .Where(c => c.AttributeLists.Any(al => al.Attributes.Any(a => a.Name.ToString() == "ExtensionMethod")));

                            foreach (var methodDeclaration in methodDeclarations)
                            {
                                var invokeParams = new ExtensionMethodInfo()
                                {
                                    Project = proj,
                                    Class = className,
                                    Method = methodDeclaration.Identifier.ToString(),
                                    Icon = VSCodeIcons.VscChecklist
                                };

                                Extensions.Add(invokeParams);
                            }
                        }
                    }
                }
            }
        }
    }
}
