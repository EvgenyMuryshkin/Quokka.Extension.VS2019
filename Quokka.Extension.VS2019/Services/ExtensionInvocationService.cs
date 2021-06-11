using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Task = System.Threading.Tasks.Task;


namespace Quokka.Extension.VS2019
{
    class ExtensionInvocationService
    {
        private readonly IExtensionLogger _logger;
        private readonly IServiceProvider _serviceProvider;
        public ExtensionInvocationService(IExtensionLogger logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        List<ExtensionMethodInfo> invokeHistory = new List<ExtensionMethodInfo>();

        public async Task RerunExtensionMethodAsync()
        {
            if (invokeHistory.Count == 0)
            {
                _logger.WriteLine($"Run history is empty");
                return;
            }

            await InvokeExtensionMethodAsync(invokeHistory[0], false);
        }

        public async Task InvokeExtensionMethodAsync(ExtensionMethodInfo _invokeParams, bool pushToMRU = true)
        {
            if (_invokeParams == null)
                return;

            try
            {
                if (pushToMRU)
                {
                    invokeHistory.RemoveAll(p => p.ToString() == _invokeParams.ToString());
                    invokeHistory.Insert(0, _invokeParams);
                }

                var invoke = $"{_invokeParams.Class}.{_invokeParams.Method}";

                _logger.WriteLine($"{invoke} started");

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
                    _logger.WriteLine(a.Data);
                };

                proc.BeginErrorReadLine();
                proc.ErrorDataReceived += (s, a) =>
                {
                    _logger.WriteLine(a.Data);
                };

                proc.WaitForExit();

                if (proc.ExitCode == 0)
                {
                    _logger.WriteLine($"{invoke} finished");
                }
                else
                {
                    _logger.WriteLine($"{invoke} failed with code: {proc.ExitCode}");
                }
            }
            catch (Exception ex)
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                var title = $"{_invokeParams.Class}.{_invokeParams.Method} failed";

                VsShellUtilities.ShowMessageBox(
                    _serviceProvider,
                    ex.Message,
                    $"{title}. See Quokka output window for details",
                    OLEMSGICON.OLEMSGICON_CRITICAL,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

                _logger.WriteLine(title);
                _logger.WriteLine(ex.Message);
                _logger.WriteLine(ex.StackTrace);
            }
        }
    }
}
