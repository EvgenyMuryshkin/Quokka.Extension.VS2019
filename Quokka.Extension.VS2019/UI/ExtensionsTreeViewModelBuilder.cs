using System;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace Quokka.Extension.VS2019
{
    public class ExtensionsTreeViewModelBuilder
    {
        public static void PopulateViewModel(ExtensionsTreeViewModel treeViewModel, Func<ExtensionMethodInfo, ICommand> commandFactory)
        {
            var svc = new ExtensionsDiscoveryService();
            svc.Reload(treeViewModel.SolutionPath);

            var path = treeViewModel.SolutionPath;
            if (File.Exists(path))
                path = Path.GetDirectoryName(path);

            treeViewModel.Projects.Clear();

            foreach (var proj in svc.Extensions.GroupBy(p => p.Project))
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
