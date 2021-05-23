using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;


namespace Quokka.Extension.VS2019
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(QuokkaExtensionVS2019Package.PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class QuokkaExtensionVS2019Package : AsyncPackage, IExtensionLogger
    {
        public static QuokkaExtensionVS2019Package Instance;
        public static Guid QuokkaOutputWindowId = Guid.Parse("51db306c-dd34-4cdf-afa7-3b35946cb4e1");

        /// <summary>
        /// Quokka.Extension.VS2019Package GUID string.
        /// </summary>
        public const string PackageGuidString = "d9ff7f11-8ffd-4154-9fb2-2d1857864b98";

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            Instance = this;

            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            SetupOutputWindow();
            await Quokka.Extension.VS2019.InvokeExtensionMethodCommand.InitializeAsync(this);
            await Quokka.Extension.VS2019.RerunExtensionMethodCommand.InitializeAsync(this);
        }
        #endregion

        IServiceProvider ServiceProvider => this as IServiceProvider;
        IVsOutputWindow OutputWindow => ServiceProvider.GetService<SVsOutputWindow, IVsOutputWindow>();
        IVsOutputWindowPane QuokkaPane { get; set; }
        void SetupOutputWindow()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            OutputWindow.GetPane(ref QuokkaOutputWindowId, out var quokkaPane);
            if (quokkaPane == null)
            {
                OutputWindow.CreatePane(ref QuokkaOutputWindowId, "Quokka", 1, 1);
                OutputWindow.GetPane(ref QuokkaOutputWindowId, out quokkaPane);
                quokkaPane.Activate();
            }

            QuokkaPane = quokkaPane;
            WriteLine("Quokka FPGA has initialized");
        }

        public void Write(string message)
        {
            QuokkaPane?.OutputStringThreadSafe(message);
        }

        public void WriteLine(string message)
        {
            Write($"{message}{Environment.NewLine}");
        }

        List<ExtensionMethodInvokeParams> invokeHistory = new List<ExtensionMethodInvokeParams>();

        public async Task RerunExtensionMethodAsync()
        {
            if (invokeHistory.Count == 0)
                return;

            await InvokeExtensionMethodAsync(invokeHistory[0], false);
        }

        public async Task InvokeExtensionMethodAsync(ExtensionMethodInvokeParams _invokeParams, bool pushToMRU = true)
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

                WriteLine($"{invoke} started");

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
                    WriteLine(a.Data);
                };

                proc.BeginErrorReadLine();
                proc.ErrorDataReceived += (s, a) =>
                {
                    WriteLine(a.Data);
                };

                proc.WaitForExit();

                if (proc.ExitCode == 0)
                {
                    WriteLine($"{invoke} finished");
                }
                else
                {
                    WriteLine($"{invoke} failed with code: {proc.ExitCode}");
                }
            }
            catch (Exception ex)
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                var title = $"{_invokeParams.Class}.{_invokeParams.Method} failed";
                VsShellUtilities.ShowMessageBox(
                    this,
                    ex.Message,
                    $"{title}. See Quokka output window for details",
                    OLEMSGICON.OLEMSGICON_CRITICAL,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

                WriteLine(title);
                WriteLine(ex.Message);
                WriteLine(ex.StackTrace);
            }
        }
    }
}
