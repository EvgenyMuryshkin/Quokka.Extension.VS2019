using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Quokka.Extension.Scaffolding;
using Quokka.Extension.ViewModels;
using System;
using System.IO;

namespace Quokka.Extension.VS2019
{
    internal class InvokeExtensionMethodCommand : ExtensionCommand
    {
        private readonly Func<ExtensionsTreeViewModel> _viewModelFactory;
        private readonly ReloadCommandViewModel.Factory _reloadCommandViewModelFactory;

        public InvokeExtensionMethodCommand(
            ExtensionDeps deps,
            Func<ExtensionsTreeViewModel> viewModelFactory,
            ReloadCommandViewModel.Factory reloadCommandViewModelFactory)
            : base(deps, guidQuokkaExtensionVS2019PackageIds.cmdidInvokeExtensionMethodCommandId)
        {
            _viewModelFactory = viewModelFactory;
            _reloadCommandViewModelFactory = reloadCommandViewModelFactory;
        }

        protected override void OnExecute()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            Solution.GetSolutionInfo(out var dir, out var file, out var opts);

            if (file == null)
                file = @"c:\code\qusoc\qusoc.sln";

            if (!File.Exists(file))
                return;

            var treeViewModel = _viewModelFactory();
            treeViewModel.Init(file);

            treeViewModel.ReloadCommand = _reloadCommandViewModelFactory(treeViewModel);

            var content = new ExtensionsTree()
            {
                DataContext = treeViewModel
            };

            var dlg = new DialogWindow()
            {
                Title = $"Solution: {file}",
                Content = content
            };

            WindowHelper.ShowModal(dlg);
        }
    }
}
