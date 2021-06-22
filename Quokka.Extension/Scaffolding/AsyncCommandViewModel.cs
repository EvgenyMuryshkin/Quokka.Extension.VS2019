using System.Threading.Tasks;

namespace Quokka.Extension.Scaffolding
{
    public abstract class AsyncCommandViewModel : CommandViewModel
    {
        public AsyncCommandViewModel(ExtensionDeps deps) : base(deps) { }

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
}
