using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;

namespace Quokka.Extension.VS2022
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("C08BAE86-A726-40CF-BBD0-AD13879D508D")]
    public class QuokkaExplorer : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuokkaExplorer"/> class.
        /// </summary>
        public QuokkaExplorer() : base(null)
        {
            this.Caption = "Quokka Explorer";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            this.Content = new ExtensionsTree();
            
            this.ToolBar = new CommandID(
                guidQuokkaExtensionVS2022PackageIds.guidQuokkaExtensionVS2022PackageCmdSet,
                guidQuokkaExtensionVS2022PackageIds.QuokkaExplorerToolbarId
            );
            this.ToolBarLocation = (int)VSTWT_LOCATION.VSTWT_TOP;
        }
    }
}
