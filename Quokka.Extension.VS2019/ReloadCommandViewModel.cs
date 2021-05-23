namespace Quokka.Extension.VS2019
{
    class ReloadCommandViewModel : CommandViewModel
    {
        private readonly ExtensionsTreeViewModel _viewModel;
        public ReloadCommandViewModel(ExtensionsTreeViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public override void Execute(object parameter)
        {
            ExtensionsTreeViewModelBuilder.PopulateViewModel(
                _viewModel,
                (invokeParams) =>
                {
                    return new ExtensionMethodInvocationCommandViewModel(QuokkaExtensionVS2019Package.Instance, invokeParams);
                });
        }
    }
}
