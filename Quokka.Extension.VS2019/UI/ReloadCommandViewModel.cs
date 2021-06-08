namespace Quokka.Extension.VS2019
{
    class ReloadCommandViewModel : CommandViewModel
    {
        private readonly ExtensionInvocationService _invocationService;
        private readonly ExtensionsTreeViewModel _viewModel;
        public ReloadCommandViewModel(ExtensionInvocationService invocationService, ExtensionsTreeViewModel viewModel)
        {
            _invocationService = invocationService;
            _viewModel = viewModel;
        }

        public override void Execute(object parameter)
        {
            ExtensionsTreeViewModelBuilder.PopulateViewModel(
                _viewModel,
                (invokeParams) =>
                {
                    return new ExtensionMethodInvocationCommandViewModel(_invocationService, invokeParams);
                });
        }
    }
}
