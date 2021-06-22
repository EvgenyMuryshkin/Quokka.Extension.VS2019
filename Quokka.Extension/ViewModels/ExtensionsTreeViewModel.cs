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

        public string SolutionPath => _ecs.Solution;
        public ObservableCollection<TreeItemViewModel> Roots { get; private set; } = new ObservableCollection<TreeItemViewModel>();

        public ExtensionsTreeViewModel(
            IExtensionsCacheService ecs,
            IExtensionNotificationService ens,
            ExtensionsTreeViewModelBuilder viewModelBuilder
            )
        {
            _ecs = ecs;
            _ens = ens;
            _viewModelBuilder = viewModelBuilder;

            _ens.OnSolutionChanged += (s, a) =>
            {
                Populate();
            };
            Populate();
        }

        void Populate()
        {
            _viewModelBuilder.PopulateViewModel(this);
        }
    }
}
