using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Quokka.Extension
{
    public class ExtensionsTreeViewModel : ViewModel
    {
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
    }
}
