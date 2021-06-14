using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Quokka.Extension.VS2019.UI.ExploreIcons;

namespace Quokka.Extension.VS2019
{
    internal class ExploreCommand : ExtensionCommand
    {
        public ExploreCommand(ExtensionDeps deps)
            : base(deps, guidQuokkaExtensionVS2019PackageIds.cmdidExploreCommand)
        {
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
