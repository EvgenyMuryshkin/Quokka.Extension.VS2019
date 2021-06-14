using Microsoft.VisualStudio.Shell;
using Quokka.Extension.Interface;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Task = System.Threading.Tasks.Task;

namespace Quokka.Extension.VS2019
{
    public abstract class CommandViewModel : ICommand
    {
        private readonly ExtensionDeps _deps;
        protected IExtensionLogger Logger => _deps.Logger;
        protected AsyncPackage ServiceProvider => _deps.ServiceProvider;
        protected IJoinableTaskFactory TaskFactory => _deps.TaskFactory;

        public CommandViewModel(ExtensionDeps deps)
        {
            _deps = deps;
        }

        public event EventHandler CanExecuteChanged;

        protected bool _running = false;
        protected bool Running
        {
            get => _running;
            set
            {
                _running = value;
                CanExecuteChanged?.Invoke(this, new EventArgs());
            }
        }
        protected async Task SetRunningAsync(bool running)
        {
            await ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                Running = running;
                return Task.CompletedTask;
            });
        }


        public virtual bool CanExecute(object parameter) => !Running;

        public abstract void Execute(object parameter);
    }
}
