using System.Collections.ObjectModel;

namespace Quokka.Extension.VS2019
{
    public class ExtensionClassViewModel : ViewModel
    {
        string _name = "Not Set";
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<ExtensionMethodViewModel> Methods { get; private set; } = new ObservableCollection<ExtensionMethodViewModel>();
    }
}
