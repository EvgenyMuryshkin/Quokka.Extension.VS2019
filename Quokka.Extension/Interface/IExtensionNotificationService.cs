using System;

namespace Quokka.Extension.Interface
{
    public interface IExtensionNotificationService
    {
        event EventHandler OnSolutionChanged;
        event EventHandler OnSolutionClosed;
        event EventHandler OnThemeChanged;
        event EventHandler OnExtensionsReloaded;

        void RaiseSolutionChanged();
        void RaiseSolutionClosed();
        void RaiseThemeChanged();
        void RaiseExtensionsReloaded();
    }
}
