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
        private readonly IExtensionNotificationService _ens;
        private readonly ExtensionsTreeViewModelBuilder _viewModelBuilder;
        private readonly IJoinableTaskFactory _taskFactory;

        public string SolutionPath => _ecs.Solution;

        ObservableCollection<TreeItemViewModel> _roots = new ObservableCollection<TreeItemViewModel>();
        public ObservableCollection<TreeItemViewModel> Roots
        {
            get => _roots;
            set
            {
                _roots = value;
                OnPropertyChanged();
            }
        }

        public ExtensionsTreeViewModel(
            IExtensionsCacheService ecs,
            IExtensionNotificationService ens,
            IJoinableTaskFactory taskFactory,
            ExtensionsTreeViewModelBuilder viewModelBuilder
            )
        {
            _ecs = ecs;
            _ens = ens;
            _viewModelBuilder = viewModelBuilder;
            _taskFactory = taskFactory;

            _ens.OnExtensionsReloaded += (s, a) =>
            {
                Populate();
            };

            _ens.OnThemeChanged += (s, a) =>
            {
                Populate();
            };

            Populate();
        }

        async void Populate()
        {
            await _taskFactory.SwitchToMainThreadAsync();
            _viewModelBuilder.PopulateViewModel(this);
        }
    }
}
