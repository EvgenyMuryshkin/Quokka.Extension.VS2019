﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Quokka.Extension.Interface;
using Quokka.Extension.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Quokka.Extension.Services
{
    public class ExtensionsDiscoveryService : IExtensionsDiscoveryService
    {
        private readonly IExtensionLogger _logger;

        public ExtensionsDiscoveryService(IExtensionLogger logger)
        {
            _logger = logger;
        }

        AttributeSyntax ExtensionMethodAttribute(SyntaxList<AttributeListSyntax> list)
        {
            return list.SelectMany(al => al.Attributes).Where(a => a.Name.ToString() == "ExtensionMethod").SingleOrDefault();
        }

        (Type, int) FetchIcon(AttributeSyntax m)
        {
            var defaultIcon = (typeof(VSCodeIcons), (int)VSCodeIcons.VscChecklist);

            if (m == null || m.ArgumentList == null)
                return defaultIcon;

            var iconSyntax = m
                .ArgumentList
                .Arguments
                .OfType<AttributeArgumentSyntax>()
                .Where(a => a.NameColon?.Name.ToString() == "icon")
                .SingleOrDefault();

            if (iconSyntax != null)
            {
                switch (iconSyntax.Expression)
                {
                    case MemberAccessExpressionSyntax maes:
                    {
                        var accessor = maes.DescendantNodes(n => true).OfType<IdentifierNameSyntax>().Select(t => t.ToString()).ToList();
                        if (accessor.Count() == 2)
                        {
                            var iconType = ExtensionCatalogue.IconTypes.SingleOrDefault(t => t.Name == accessor[0]);
                            if (iconType != null)
                            {
                                var enumNames = Enum.GetNames(iconType).ToList();
                                var enumValues = Enum.GetValues(iconType);
                                var valueIndex = enumNames.IndexOf(accessor[1]);
                                var value = enumValues.GetValue(valueIndex);

                                return (iconType, (int)value);
                            }
                        }
                    }
                    break;
                }
            }

            return defaultIcon;
        }

        public List<ExtensionMethodInfo> LoadFromDirectory(string solutionPath)
        {
            var extensions = new List<ExtensionMethodInfo>();

            if (File.Exists(solutionPath))
                solutionPath = Path.GetDirectoryName(solutionPath);

            if (!Directory.Exists(solutionPath))
                return extensions;

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
                                .Where(m => ExtensionMethodAttribute(m.AttributeLists) != null);

                            foreach (var methodDeclaration in methodDeclarations)
                            {
                                var methodAttr = ExtensionMethodAttribute(methodDeclaration.AttributeLists);
                                var (iconType, iconValue) = FetchIcon(methodAttr);

                                var invokeParams = new ExtensionMethodInfo()
                                {
                                    Project = proj,
                                    Class = className,
                                    Method = methodDeclaration.Identifier.ToString(),
                                    Icon = new ExtensionMethodIcon(iconType, iconValue)
                                };

                                extensions.Add(invokeParams);
                                _logger.WriteLine($"Found extension method: {invokeParams.Class}.{invokeParams.Method}");
                            }
                        }
                    }
                }
            }

            return extensions;
        }
    }
}