using Quokka.Extension.Interface;
using System;

namespace Quokka.Extension.Services
{
    public class ExtensionNotificationService : IExtensionNotificationService
    {
        public event EventHandler<SolutionChangedEventArgs> OnSolutionChanged;

        public void RaiseSolutionChanged(string solution)
        {
            var handler = OnSolutionChanged;
            handler?.Invoke(this, new SolutionChangedEventArgs(solution));
        }
    }
}
