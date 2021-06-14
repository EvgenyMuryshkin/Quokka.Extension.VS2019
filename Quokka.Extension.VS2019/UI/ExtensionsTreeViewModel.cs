using Quokka.Extension.Interface;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Quokka.Extension.VS2019
{
    public class ExtensionsTreeViewModel : ViewModel
    {
        private readonly IExtensionInvocationService _invocationService;
        private readonly IExtensionsCacheService _ecs;
        private readonly ExtensionsTreeViewModelBuilder _viewModelBuilder;
        private readonly ExtensionMethodInvocationCommandViewModel.Factory _extensionMethodInvocationCommandViewModelFactory;

        string _solutionPath = "Not Set";
        public string SolutionPath
        {
            get => _solutionPath;
            set
            {
                _solutionPath = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ExtensionProjectViewModel> Projects { get; private set; } = new ObservableCollection<ExtensionProjectViewModel>();

        ICommand _reload;
        public ICommand ReloadCommand
        {
            get => _reload;
            set
            {
                _reload = value;
                OnPropertyChanged();
            }
        }

        public ExtensionsTreeViewModel(
            IExtensionInvocationService invocationService, 
            IExtensionsCacheService ecs, 
            ExtensionsTreeViewModelBuilder viewModelBuilder,
            ExtensionMethodInvocationCommandViewModel.Factory extensionMethodInvocationCommandViewModelFactory)
        {
            _invocationService = invocationService;
            _ecs = ecs;
            _viewModelBuilder = viewModelBuilder;
            _extensionMethodInvocationCommandViewModelFactory = extensionMethodInvocationCommandViewModelFactory;
            ReloadCommand = new Command(() => this.Reload());
        }

        public void Init(string solutionPath)
        {
            SolutionPath = solutionPath;
            Reload();
        }

        public void Reload()
        {
            _ecs.Reload(SolutionPath);
            _viewModelBuilder.PopulateViewModel(
                this, 
                (invokeParams) =>
                {
                    return _extensionMethodInvocationCommandViewModelFactory(invokeParams);
                });
        }
    }
}
