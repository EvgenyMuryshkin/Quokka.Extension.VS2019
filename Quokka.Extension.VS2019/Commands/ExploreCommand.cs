using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Quokka.Extension.VS2019.UI;
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
    internal class ExploreCommand : ExtensionCommand
    {
        private readonly ExtensionInvocationService _invocationService;

        public ExploreCommand(ExtensionPartDeps deps, ExtensionInvocationService invocationService)
            : base(deps, guidQuokkaExtensionVS2019PackageIds.cmdidExploreCommand)
        {
            _invocationService = invocationService;
        }

        protected override void OnExecute()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var dlg = new DialogWindow()
            {
                Title = $"Explore icons",
                Content = new ExploreIcons()
            };

            WindowHelper.ShowModal(dlg);

        }
    }
}
