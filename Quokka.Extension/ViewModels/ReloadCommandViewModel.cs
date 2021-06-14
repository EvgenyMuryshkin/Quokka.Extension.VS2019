using Quokka.Extension.Scaffolding;

namespace Quokka.Extension.ViewModels
{
    public class ReloadCommandViewModel : CommandViewModel
    {
        private readonly ExtensionsTreeViewModel _viewModel;

        public delegate ReloadCommandViewModel Factory(ExtensionsTreeViewModel viewModel);

        public ReloadCommandViewModel(ExtensionDeps deps, ExtensionsTreeViewModel viewModel) : base(deps)
        {
            _viewModel = viewModel;
        }

        public override void Execute(object parameter)
        {
            _viewModel.Reload();
        }
    }
}
