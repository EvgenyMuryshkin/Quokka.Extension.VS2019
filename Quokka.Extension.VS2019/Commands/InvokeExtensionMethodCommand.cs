using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Task = System.Threading.Tasks.Task;

namespace Quokka.Extension.VS2019
{
    internal class InvokeExtensionMethodCommand : ExtensionCommand
    {
        private readonly ExtensionInvocationService _invocationService;

        public InvokeExtensionMethodCommand(ExtensionPartDeps deps, ExtensionInvocationService invocationService)
            : base(deps, guidQuokkaExtensionVS2019PackageIds.cmdidInvokeExtensionMethodCommandId)
        {
            _invocationService = invocationService;
        }

        protected override void OnExecute()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            Solution.GetSolutionInfo(out var dir, out var file, out var opts);

            if (file == null)
                file = @"c:\code\qusoc\qusoc.sln";

            if (!File.Exists(file))
                return;

            var treeViewModel = new ExtensionsTreeViewModel()
            {
                SolutionPath = file,
            };
            treeViewModel.ReloadCommand = new ReloadCommandViewModel(_invocationService, treeViewModel);
            treeViewModel.ReloadCommand.Execute(null);

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
