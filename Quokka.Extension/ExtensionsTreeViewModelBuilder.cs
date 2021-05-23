using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Xml.Linq;

namespace Quokka.Extension
{
    public class ExtensionMethodInvokeParams
    {
        public string Project;
        public string Class;
        public string Method;
    }

    public class ExtensionsTreeViewModelBuilder
    {
        public static void PopulateViewModel(ExtensionsTreeViewModel treeViewModel, Func<ExtensionMethodInvokeParams, ICommand> commandFactory)
        {
            var path = treeViewModel.SolutionPath;
            if (File.Exists(path))
                path = Path.GetDirectoryName(path);

            treeViewModel.Projects.Clear();

            var projFiles = Directory.EnumerateFiles(path, "*.csproj", SearchOption.AllDirectories);

            foreach (var proj in projFiles)
            {
                var xProject = XDocument.Parse(File.ReadAllText(proj));
                var refs = xProject.Root
                    .Elements("ItemGroup")
                    .SelectMany(g => g.Elements("PackageReference"))
                    .Where(p => p.Attribute("Include")?.Value == "Quokka.Extension.Interop");

                if (refs.Any())
                {
                    var projectViewModel = new ExtensionProjectViewModel()
                    {
                        Path = proj
                    };

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
                            var classViewModel = new ExtensionClassViewModel()
                            {
                                Name = classDeclaration.Identifier.ToString()
                            };

                            var methodDeclarations = classDeclaration
                                .DescendantNodes(n => true)
                                .OfType<MethodDeclarationSyntax>()
                                .Where(c => c.AttributeLists.Any(al => al.Attributes.Any(a => a.Name.ToString() == "ExtensionMethod")));

                            foreach (var methodDeclaration in methodDeclarations)
                            {
                                var invokeParams = new ExtensionMethodInvokeParams() 
                                { 
                                    Project = proj, 
                                    Class = classViewModel.Name,
                                    Method = methodDeclaration.Identifier.ToString()
                                };

                                var methodViewModel = new ExtensionMethodViewModel()
                                {
                                    Name = invokeParams.Method,
                                    InvokeCommand = commandFactory?.Invoke(invokeParams)
                                };

                                classViewModel.Methods.Add(methodViewModel);
                            }

                            projectViewModel.Classes.Add(classViewModel);
                        }

                    }

                    treeViewModel.Projects.Add(projectViewModel);
                }
            }
        }
    }
}
