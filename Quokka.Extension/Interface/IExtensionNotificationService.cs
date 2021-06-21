using System;

namespace Quokka.Extension.Interface
{
    public class SolutionChangedEventArgs : EventArgs
    {
        public string Solution { get; private set; }
        public SolutionChangedEventArgs(string solution)
        {
            Solution = solution;
        }
    }

    public interface IExtensionNotificationService
    {
        event EventHandler<SolutionChangedEventArgs> OnSolutionChanged;
        void RaiseSolutionChanged(string solution);
    }
}
