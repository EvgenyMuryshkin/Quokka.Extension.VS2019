using Quokka.Extension.Interface;
using Quokka.Extension.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Quokka.Extension.Services
{
    public class ExtensionInvocationService : IExtensionInvocationService
    {
        private readonly IInvocationCacheService _invocationCache;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly IExtensionLogger _logger;

        public ExtensionInvocationService(IExceptionHandler exceptionHandler, IExtensionLogger logger, IInvocationCacheService invocationCache)
        {
            _exceptionHandler = exceptionHandler;
            _logger = logger;
            _invocationCache = invocationCache;
        }

        public event EventHandler InvocationEvent;

        Process _runningProcess;
        Process RunningProcess
        {
            get => _runningProcess;
            set
            {
                _runningProcess = value;
                InvocationEvent?.Invoke(this, new EventArgs());
            }
        }

        public bool IsRunning => _runningProcess != null;

        public void Clear()
        {
            CancelRun();
            _invocationCache.Clear();
        }

        public void CancelRun()
        {
            var proc = RunningProcess;
            if (proc == null || proc.HasExited)
                return;

            proc.Kill();
        }

        public async Task RerunExtensionMethodAsync()
        {
            var peek = _invocationCache.Peek();

            if (peek == null)
            {
                _logger.WriteLine($"Run history is empty");
                return;
            }

            await InvokeExtensionMethodAsync(peek, false);
        }

        public async Task InvokeExtensionMethodAsync(ExtensionMethodInfo _invokeParams, bool pushToMRU = true)
        {
            if (_invokeParams == null)
                return;

            // limit to single method run for now
            if (RunningProcess != null)
                return;

            try
            {
                if (pushToMRU)
                {
                    _invocationCache.Push(_invokeParams);
                }

                var invoke = $"{_invokeParams.Class}.{_invokeParams.Method}";

                await _logger.Activate();
                _logger.WriteLine($"{invoke} started");

                RunningProcess = Process.Start(new ProcessStartInfo()
                {
                    FileName = @"dotnet",
                    Arguments = $"run -- {invoke}",
                    UseShellExecute = false,
                    WorkingDirectory = Path.GetDirectoryName(_invokeParams.Project),
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                });

                RunningProcess.BeginOutputReadLine();
                RunningProcess.OutputDataReceived += (s, a) =>
                {
                    _logger.WriteLine(a.Data);
                };

                RunningProcess.BeginErrorReadLine();
                RunningProcess.ErrorDataReceived += (s, a) =>
                {
                    _logger.WriteLine(a.Data);
                };

                RunningProcess.WaitForExit();

                if (RunningProcess.ExitCode == 0)
                {
                    _logger.WriteLine($"{invoke} finished");
                }
                else
                {
                    _logger.WriteLine($"{invoke} failed with code: {RunningProcess.ExitCode}");
                }
            }
            catch (Exception ex)
            {
                var title = $"{_invokeParams.Class}.{_invokeParams.Method} failed";

                await _exceptionHandler.OnException(title, ex);
            }
            finally
            {
                RunningProcess = null;
            }
        }
    }
}
