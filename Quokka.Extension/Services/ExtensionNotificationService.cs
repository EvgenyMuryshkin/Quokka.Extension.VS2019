using Quokka.Extension.Interface;
using System;

namespace Quokka.Extension.Services
{
    public class ExtensionNotificationService : IExtensionNotificationService
    {
        public event EventHandler OnSolutionChanged;
        public event EventHandler OnSolutionClosed;
        public event EventHandler OnThemeChanged;
        public event EventHandler OnExtensionsReloaded;

        public void RaiseExtensionsReloaded()
        {
            var handler = OnExtensionsReloaded;
            handler?.Invoke(this, new EventArgs());
        }

        public void RaiseSolutionChanged()
        {
            var handler = OnSolutionChanged;
            handler?.Invoke(this, new EventArgs());
        }

        public void RaiseSolutionClosed()
        {
            var handler = OnSolutionClosed;
            handler?.Invoke(this, new EventArgs());
        }

        public void RaiseThemeChanged()
        {
            var handler = OnThemeChanged;
            handler?.Invoke(this, new EventArgs());
        }
    }
}
