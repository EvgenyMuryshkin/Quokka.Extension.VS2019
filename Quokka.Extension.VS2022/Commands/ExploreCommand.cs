using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Quokka.Extension.Interface;
using Quokka.Extension.Scaffolding;
using Quokka.Extension.VS2022.UI.ExploreIcons;
using System;

namespace Quokka.Extension.VS2022
{
    internal class ExploreCommand : ExtensionCommand
    {
        private readonly IExtensionInvocationService _invocationService;

        public ExploreCommand(ExtensionDeps deps, IExtensionInvocationService invocationService)
            : base(deps, guidQuokkaExtensionVS2022PackageIds.cmdidExploreCommand)
        {
            _invocationService = invocationService;
            _invocationService.InvocationEvent += OnInvocationEvent;
        }

        void OnInvocationEvent(object sender, EventArgs arg)
        {
            GetCommand.Enabled = !_invocationService.IsRunning;
        }

        protected override void OnExecute()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var dlg = new DialogWindow()
            {
                Title = $"Explore icons",
                Content = new ExploreIconsView()
            };

            WindowHelper.ShowModal(dlg);
        }
    }
}
