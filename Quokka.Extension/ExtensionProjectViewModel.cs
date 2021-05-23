﻿using System.Collections.ObjectModel;

namespace Quokka.Extension
{
    public class ExtensionProjectViewModel : ViewModel
    {
        string _path = "Not Set";
        public string Path
        {
            get => _path;
            set
            {
                _path = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ExtensionClassViewModel> Classes { get; private set; } = new ObservableCollection<ExtensionClassViewModel>();
    }
}