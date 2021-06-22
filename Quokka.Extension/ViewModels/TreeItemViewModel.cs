using Quokka.Extension.Scaffolding;
using System.Collections.Generic;
using System.Windows.Input;

namespace Quokka.Extension.ViewModels
{
    public class TreeItemViewModel : ViewModel
    {
        public string Name { get; set; }
        public List<TreeItemViewModel> Children { get; set; } = new List<TreeItemViewModel>();

        public ICommand MouseDoubleClickCommand { get; set; }

        bool _isExpanded = false;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    OnPropertyChanged();
                }
            }
        }

        bool _isSelected = false;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        public object ImageSource { get; set; }
        public bool HasImage => ImageSource != null;
    }
}
