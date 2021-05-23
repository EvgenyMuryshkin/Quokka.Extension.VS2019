using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Diagnostics;
using System.IO;
using Task = System.Threading.Tasks.Task;

namespace Quokka.Extension.VS2019
{
    class ExtensionMethodInvocationCommandViewModel : AsyncCommandViewModel
    {
        private readonly QuokkaExtensionVS2019Package _package;
        private readonly ExtensionMethodInvokeParams _invokeParams;
        public ExtensionMethodInvocationCommandViewModel(QuokkaExtensionVS2019Package package, ExtensionMethodInvokeParams invokeParams)
        {
            _package = package;
            _invokeParams = invokeParams;
        }

        protected override async Task ExecuteAsync(object parameter)
        {
            try
            {
                var invoke = $"{_invokeParams.Class}.{_invokeParams.Method}";

                await SetRunningAsync(true);

                var proc = Process.Start(new ProcessStartInfo()
                {
                    FileName = @"dotnet",
                    Arguments = $"run -- {invoke}",
                    UseShellExecute = false,
                    WorkingDirectory = Path.GetDirectoryName(_invokeParams.Project),
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                });

                proc.BeginOutputReadLine();
                proc.OutputDataReceived += (s, a) =>
                {
                    _package?.WriteLine(a.Data);
                };

                proc.BeginErrorReadLine();
                proc.ErrorDataReceived += (s, a) =>
                {
                    _package?.WriteLine(a.Data);
                };

                proc.WaitForExit();

                if (proc.ExitCode == 0)
                {
                    _package?.WriteLine($"{invoke} finished");
                }
                else
                {
                    _package?.WriteLine($"{invoke} failed with code: {proc.ExitCode}");
                }
            }
            catch (Exception ex)
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                var title = $"{_invokeParams.Class}.{_invokeParams.Method} failed";
                VsShellUtilities.ShowMessageBox(
                    _package,
                    ex.Message,
                    $"{title}. See Quokka output window for details",
                    OLEMSGICON.OLEMSGICON_CRITICAL,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

                _package?.WriteLine(title);
                _package?.WriteLine(ex.Message);
                _package?.WriteLine(ex.StackTrace);
            }
        }
    }
}
