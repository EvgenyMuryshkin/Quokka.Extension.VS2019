using EnvDTE;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
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

            VSColorTheme.ThemeChanged += (a) =>
            {
                _ecs.Reload();
            };

#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
            var dte = _serviceProvider.GetService(typeof(DTE)) as DTE;
            if (dte != null)
            {
                dte.Events.SolutionEvents.Opened += () =>
                {
                    _ecs.Reload(dte.Solution.FullName);
                };

                dte.Events.SolutionEvents.BeforeClosing += () =>
                {
                    _ecs.Close();
                };
            }
#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
        }
    }
}
