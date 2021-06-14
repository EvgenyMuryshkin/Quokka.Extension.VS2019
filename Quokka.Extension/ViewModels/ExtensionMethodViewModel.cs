using Quokka.Extension.Scaffolding;
using System.Windows.Input;

namespace Quokka.Extension.ViewModels
{
    public class ExtensionMethodViewModel : ViewModel
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

        ICommand _invoke;
        public ICommand InvokeCommand
        {
            get => _invoke;
            set
            {
                _invoke = value;
                OnPropertyChanged();
            }
        }
    }
}
