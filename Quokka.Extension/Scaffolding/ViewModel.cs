using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Quokka.Extension.Scaffolding
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Command : ICommand
    {
        public event EventHandler CanExecuteChanged;
        Action _action = null;
        Action<object> _actionWithParameter = null;

        public Command(Action action)
        {
            _action = action;
        }
        public Command(Action<object> actionWithParameter)
        {
            _actionWithParameter = actionWithParameter;
        }

        public bool CanExecute(object parameter)
        {
            // TODO: implement
            return true;
        }

        public void Execute(object parameter)
        {
            _action?.Invoke();
            _actionWithParameter?.Invoke(parameter);
        }
    }
}
