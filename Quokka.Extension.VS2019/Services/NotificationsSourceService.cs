using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Events;
using Quokka.Extension.Interface;
using Quokka.Extension.Scaffolding;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Quokka.Extension.VS2019.Services
{
    class NotificationsSourceService : ExtensionPart, IExtensionPart
    {
        private readonly IExtensionsCacheService _ecs;

        public NotificationsSourceService(ExtensionDeps deps, IExtensionsCacheService ecs) : base(deps)
        {
            _ecs = ecs;
        }

        protected override async Task OnInitializeAsync()
        {
            await TaskFactory.SwitchToMainThreadAsync();

#pragma warning disable VSTHRD101 // Avoid unsupported async delegates
            VSColorTheme.ThemeChanged += async (a) =>
            {
                try
                {
                    await _ecs.Reload();
                }
                catch
                {

                }
            };

            SolutionEvents.OnAfterOpenSolution += async (s, a) =>
            {
                try
                {
                    await _ecs.Reload();
                }
                catch
                {

                }
            };

            SolutionEvents.OnBeforeCloseSolution += async (s, a) =>
            {
                try
                {
                    _ecs.Close();
                }
                catch
                {

                }
            };
#pragma warning restore VSTHRD101 // Avoid unsupported async delegates
        }
    }
}
