using Quokka.Extension.Interface;
using Quokka.Extension.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace Quokka.Extension.Services
{
    class ExtensionsTreeViewModelState
    {
        Dictionary<string, bool> _visibilityMap = new Dictionary<string, bool>();
        Stack<string> _hierarchy = new Stack<string>();

        void RegisterItem(TreeItemViewModel vm)
        {
            _hierarchy.Push(vm.Name);

            _visibilityMap[string.Join(".",_hierarchy).ToLower()] = vm.IsExpanded;

            foreach (var child in vm.Children)
            {
                RegisterItem(child);
            }

            _hierarchy.Pop();
        }

        public ExtensionsTreeViewModelState(ExtensionsTreeViewModel treeViewModel)
        {
            foreach (var r in treeViewModel.Roots)
            {
                RegisterItem(r);
            }
        }

        public bool IsExpanded(Stack<string> key, bool defaultExpanded)
        {
            var check = string.Join(".", key).ToLower();
            return _visibilityMap.TryGetValue(check, out var existing) ? existing : defaultExpanded;
        }
    }

    public class ExtensionsTreeViewModelBuilder
    {
        private readonly IExtensionIconResolver _extensionIconResolver;
        private readonly IExtensionsDiscoveryService _eds;
        private readonly IExtensionsCacheService _ecs;
        private readonly ExtensionMethodInvocationCommandViewModel.Factory _extensionMethodInvocationCommandViewModelFactory;

        public ExtensionsTreeViewModelBuilder(
            IExtensionIconResolver extensionIconResolver, 
            IExtensionsDiscoveryService eds,
            IExtensionsCacheService ecs,
            ExtensionMethodInvocationCommandViewModel.Factory extensionMethodInvocationCommandViewModelFactory)
        {
            _extensionIconResolver = extensionIconResolver;
            _eds = eds;
            _ecs = ecs;
            _extensionMethodInvocationCommandViewModelFactory = extensionMethodInvocationCommandViewModelFactory;
        }

        public void PopulateViewModel(ExtensionsTreeViewModel treeViewModel)
        {
            var extensions = _ecs.Extensions;

            var path = treeViewModel.SolutionPath;
            if (File.Exists(path))
                path = Path.GetDirectoryName(path);

            var state = new ExtensionsTreeViewModelState(treeViewModel);

            var roots = new ObservableCollection<TreeItemViewModel>();

            var projectsGroup = extensions.GroupBy(p => p.Project);
            var hierarchy = new Stack<string>();

            foreach (var p in projectsGroup)
            {
                hierarchy.Push(Path.GetFileName(p.Key));

                var pItem = new TreeItemViewModel() 
                { 
                    Name = hierarchy.Peek(), 
                    IsExpanded = state.IsExpanded(hierarchy, projectsGroup.Count() == 1)
                };

                var classesGroup = p.GroupBy(c => c.Class);
                foreach (var c in classesGroup)
                {
                    hierarchy.Push(c.Key);

                    var cItem = new TreeItemViewModel() 
                    { 
                        Name = hierarchy.Peek(), 
                        IsExpanded = state.IsExpanded(hierarchy, classesGroup.Count() == 1)
                    };

                    foreach (var m in c)
                    {
                        hierarchy.Push(m.DisplayMethod);

                        var mItem = new TreeItemViewModel()
                        {
                            Name = hierarchy.Peek(),
                            ImageSource = _extensionIconResolver.Resolve(m.Icon),
                            MouseDoubleClickCommand = _extensionMethodInvocationCommandViewModelFactory?.Invoke(m)
                        };
                        cItem.Children.Add(mItem);
                        hierarchy.Pop();
                    }

                    pItem.Children.Add(cItem);
                    hierarchy.Pop();
                }

                roots.Add(pItem);
                hierarchy.Pop();
            }

            treeViewModel.Roots = roots;
        }
    }
}
