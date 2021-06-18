using Quokka.Extension.Interface;
using Quokka.Extension.Scaffolding;
using Quokka.Extension.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Quokka.Extension.ViewModels
{
    public class ExtensionsTreeViewModel : ViewModel
    {
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

        public ObservableCollection<TreeItemViewModel> Roots { get; private set; } = new ObservableCollection<TreeItemViewModel>();

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
            IExtensionsCacheService ecs, 
            ExtensionsTreeViewModelBuilder viewModelBuilder,
            ExtensionMethodInvocationCommandViewModel.Factory extensionMethodInvocationCommandViewModelFactory)
        {
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
