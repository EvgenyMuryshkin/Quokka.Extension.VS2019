using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Events;
using Quokka.Extension.Interface;
using Quokka.Extension.Scaffolding;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Quokka.Extension.VS2022.Services
{
    class NotificationsSourceService : ExtensionPart, IExtensionPart
    {
        public NotificationsSourceService(ExtensionDeps deps) : base(deps)
        {
        }

        protected override async Task OnInitializeAsync()
        {
            await _taskFactory.SwitchToMainThreadAsync();
            VSColorTheme.ThemeChanged += (a) => _ens.RaiseThemeChanged();
            SolutionEvents.OnAfterOpenSolution += (s, a) => _ens.RaiseSolutionChanged();
            SolutionEvents.OnBeforeCloseSolution += (s, a) => _ens.RaiseSolutionClosed();
        }
    }
}
