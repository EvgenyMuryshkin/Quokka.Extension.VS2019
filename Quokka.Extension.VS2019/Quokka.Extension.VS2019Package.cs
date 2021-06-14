﻿using Autofac;
using Autofac.Features.ResolveAnything;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Quokka.Extension.Interface;
using Quokka.Extension.Services;
using System;
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
    [Guid(guidQuokkaExtensionVS2019PackageIds.PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(UIContextGuids80.NoSolution, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class QuokkaExtensionVS2019Package : AsyncPackage, IExceptionHandler, IJoinableTaskFactory
    {
        public static QuokkaExtensionVS2019Package Instance;
        private IContainer _container;

        #region Package Members

        void ConfigureContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());

            // singletons
            builder.RegisterInstance<AsyncPackage>(Instance)
                .AsSelf()
                .As<AsyncPackage>()
                .As<IExceptionHandler>()
                .As<IServiceProvider>()
                .As<IJoinableTaskFactory>()
                ;

            builder.RegisterType<QuokkaOutputWindowExtensionPart>()
                .SingleInstance()
                .AsSelf()
                .As<IExtensionLogger>()
                ;

            builder.RegisterType<ExtensionsCacheService>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ExtensionInvocationService>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<InvocationCacheService>().AsImplementedInterfaces().SingleInstance();

            // transient
            builder.RegisterType<ExtensionsDiscoveryService>().AsImplementedInterfaces();

            _container = builder.Build();
        }

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

            var commands = new[]
            {
                typeof(QuokkaOutputWindowExtensionPart),
                typeof(InvokeExtensionMethodCommand),
                typeof(RerunExtensionMethodCommand),
                typeof(CancelRunMethodCommand),
                typeof(ExploreCommand)
            };

            try
            {
                ConfigureContainer();

                foreach (var commandType in commands)
                {
                    var instance = _container.Resolve(commandType) as IExtensionPart;
                    if (instance == null) throw new Exception($"Type {commandType} is not extension part");

                    await instance.InitializeAsync();
                }

                new DynamicMenu(guidQuokkaExtensionVS2019PackageIds.cmdidMyDynamicStart_Translate, this, 1);
                new DynamicMenu(guidQuokkaExtensionVS2019PackageIds.cmdidMyDynamicStart_BitStream, this, 2);
                new DynamicMenu(guidQuokkaExtensionVS2019PackageIds.cmdidMyDynamicStart_Program, this, 3);

                new DynamicMenu(guidQuokkaExtensionVS2019PackageIds.cmdidMyDynamicStartCommand1, this, 4);
                new DynamicMenu(guidQuokkaExtensionVS2019PackageIds.cmdidMyDynamicStartCommand2, this, 0);
                new DynamicMenu(guidQuokkaExtensionVS2019PackageIds.cmdidMyDynamicStartCommand3, this, 0);

                var ecs = _container.Resolve<IExtensionsCacheService>();
                ecs.Reload(@"c:\code\qusoc\qusoc.sln");

                CompleteInitialization();
            }
            catch (Exception ex)
            {
                VsShellUtilities.ShowMessageBox(
                    this,
                    ex.ToString(),
                    ex.Message,
                    OLEMSGICON.OLEMSGICON_CRITICAL,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

            }
        }
        #endregion

        IServiceProvider ServiceProvider => this;
        IVsOutputWindow OutputWindow => ServiceProvider.GetService<SVsOutputWindow, IVsOutputWindow>();
        IVsOutputWindowPane QuokkaPane { get; set; }

        void CompleteInitialization()
        {
            OutputWindow.GetPane(ref guidQuokkaExtensionVS2019PackageIds.QuokkaOutputWindowId, out var quokkaPane);
            QuokkaPane = quokkaPane;
            WriteLine("Quokka FPGA has initialized");
        }

        public void Write(string message) => QuokkaPane?.OutputStringThreadSafe(message);
        public void WriteLine(string message) => Write($"{message}{Environment.NewLine}");

        public async Task OnException(string title,Exception ex)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

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

        public async Task Run(Func<Task> asyncMethod)
        {
            await ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await asyncMethod();
                return Task.CompletedTask;
            });
        }

        public async Task SwitchToMainThreadAsync(CancellationToken cancellationToken = default)
        {
            // how will that work...
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        }
    }
}
