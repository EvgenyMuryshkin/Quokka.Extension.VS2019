using Quokka.Extension.Interface;
using Quokka.Extension.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace Quokka.Extension.Services
{
    public class ExtensionsTreeViewModelBuilder
    {
        private readonly IExtensionIconResolver _extensionIconResolver;
        private readonly IExtensionsDiscoveryService _eds;
        public ExtensionsTreeViewModelBuilder(IExtensionIconResolver extensionIconResolver, IExtensionsDiscoveryService eds)
        {
            _extensionIconResolver = extensionIconResolver;
            _eds = eds;
        }

        public void PopulateViewModel(ExtensionsTreeViewModel treeViewModel, Func<ExtensionMethodInfo, ICommand> commandFactory)
        {
            var extensions = _eds.LoadFromDirectory(treeViewModel.SolutionPath);

            var path = treeViewModel.SolutionPath;
            if (File.Exists(path))
                path = Path.GetDirectoryName(path);

            treeViewModel.Roots.Clear();

            var projectsGroup = extensions.GroupBy(p => p.Project);
            foreach (var p in projectsGroup)
            {
                var pItem = new TreeItemViewModel() { Name = p.Key, IsExpanded = projectsGroup.Count() == 1 };

                var classesGroup = p.GroupBy(c => c.Class);
                foreach (var c in classesGroup)
                {
                    var cItem = new TreeItemViewModel() { Name = c.Key, IsExpanded = classesGroup.Count() == 1 };

                    foreach (var m in c)
                    {
                        var mItem = new TreeItemViewModel()
                        {
                            Name = m.Method,
                            ImageSource = _extensionIconResolver.Resolve(m.Icon),
                            MouseDoubleClickCommand = commandFactory?.Invoke(m)
                        };
                        cItem.Children.Add(mItem);
                    }

                    pItem.Children.Add(cItem);
                }

                treeViewModel.Roots.Add(pItem);
            }
        }
    }
}
