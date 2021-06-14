using Quokka.Extension.Interface;
using Quokka.Extension.Services;
using Quokka.Extension.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace Quokka.Extension.Services
{
    public class ExtensionsTreeViewModelBuilder
    {
        private readonly IExtensionsDiscoveryService _eds;
        public ExtensionsTreeViewModelBuilder(IExtensionsDiscoveryService eds)
        {
            _eds = eds;
        }

        public void PopulateViewModel(ExtensionsTreeViewModel treeViewModel, Func<ExtensionMethodInfo, ICommand> commandFactory)
        {
            var extensions = _eds.LoadFromDirectory(treeViewModel.SolutionPath);

            var path = treeViewModel.SolutionPath;
            if (File.Exists(path))
                path = Path.GetDirectoryName(path);

            treeViewModel.Projects.Clear();

            foreach (var proj in extensions.GroupBy(p => p.Project))
            {
                var projectViewModel = new ExtensionProjectViewModel()
                {
                    Path = proj.Key
                };

                foreach (var classGroup in proj.GroupBy(c => c.Class))
                {
                    var classViewModel = new ExtensionClassViewModel()
                    {
                        Name = classGroup.Key
                    };

                    foreach (var method in classGroup)
                    {
                        var methodViewModel = new ExtensionMethodViewModel()
                        {
                            Name = method.Method,
                            InvokeCommand = commandFactory?.Invoke(method)
                        };

                        classViewModel.Methods.Add(methodViewModel);
                    }

                    projectViewModel.Classes.Add(classViewModel);
                }

                treeViewModel.Projects.Add(projectViewModel);
            }
        }
    }
}
