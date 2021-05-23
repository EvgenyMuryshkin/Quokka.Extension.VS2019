using Microsoft.VisualStudio.Shell;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Task = System.Threading.Tasks.Task;

namespace Quokka.Extension.VS2019
{
    public abstract class AsyncCommandViewModel : CommandViewModel
    {
        protected abstract Task ExecuteAsync(object parameter);

        public override void Execute(object parameter)
        {
            if (Running)
                return;

            Running = true;

            Task.Factory.StartNew(async () =>
            {
                try
                {
                    await SetRunningAsync(true);
                    await ExecuteAsync(parameter);
                }
                finally
                {
                    await SetRunningAsync(false);
                }
            });
        }
    }

    public abstract class CommandViewModel : ICommand
    {
        public CommandViewModel()
        {

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
