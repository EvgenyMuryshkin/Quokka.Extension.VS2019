using Quokka.Extension.Interface;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Quokka.Extension.Scaffolding
{
    public abstract class CommandViewModel : ICommand
    {
        private readonly ExtensionDeps _deps;
        protected IExtensionLogger Logger => _deps.Logger;
        protected IServiceProvider ServiceProvider => _deps.ServiceProvider;
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
            await TaskFactory.Run(async () =>
            {
                await TaskFactory.SwitchToMainThreadAsync();
                Running = running;
                return Task.CompletedTask;
            });
        }


        public virtual bool CanExecute(object parameter) => !Running;

        public abstract void Execute(object parameter);
    }
}
