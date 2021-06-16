using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using Quokka.Extension.Interface;
using Quokka.Extension.Scaffolding;
using Quokka.Extension.Interop;
using Quokka.Extension.Services;
using System.Collections.Generic;
using System.Linq;
using Task = System.Threading.Tasks.Task;
using System.Threading.Tasks;

namespace Quokka.Extension.VS2019
{
    public class DynamicExtensionMethodsMenuService : ExtensionService, IExtensionPart
    {
        private readonly IExtensionsCacheService _extensionsCacheService;
        private readonly IExtensionInvocationService _invocationService;

        private readonly uint _cmdidMyDynamicStartCommand;
        private readonly ExtensionMethodIcon _icon;

        List<ExtensionMethodInfo> _matchingMethods = new List<ExtensionMethodInfo>();
        private int _maxCount => _matchingMethods.Count;

        public delegate DynamicExtensionMethodsMenuService Factory(uint cmdidMyDynamicStartCommand, ExtensionMethodIcon icon);

        public DynamicExtensionMethodsMenuService(
            ExtensionDeps deps,
            IExtensionInvocationService invocationService,
            IExtensionsCacheService extensionsCacheService,
            uint cmdidMyDynamicStartCommand, 
            ExtensionMethodIcon icon
            ) : base(deps)
        {
            _invocationService = invocationService;
            _extensionsCacheService = extensionsCacheService;
            _cmdidMyDynamicStartCommand = cmdidMyDynamicStartCommand;
            _icon = icon;
        }

        public Task InitializeAsync()
        {
            _matchingMethods = _extensionsCacheService.ExtensionsForIcon(_icon);

            OleMenuCommandService commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                // Add the DynamicItemMenuCommand for the expansion of the root item into N items at run time.
                CommandID dynamicItemRootId = new CommandID(guidQuokkaExtensionVS2019PackageIds.guidQuokkaExtensionVS2019PackageCmdSet, (int)_cmdidMyDynamicStartCommand);
                DynamicItemMenuCommand dynamicMenuCommand = new DynamicItemMenuCommand(
                    dynamicItemRootId,
                    IsValidDynamicItem,
                    OnInvokedDynamicItem,
                    OnBeforeQueryStatusDynamicItem)
                {
                    Visible = false
                };
                commandService.AddCommand(dynamicMenuCommand);
            }

            return Task.CompletedTask;
        }

        private void OnInvokedDynamicItem(object sender, EventArgs args)
        {
            DynamicItemMenuCommand invokedCommand = (DynamicItemMenuCommand)sender;

            var extension = _matchingMethods[invokedCommand.DynamicIndex];

            Logger.WriteLine($"Icon clicked: {invokedCommand.DynamicIndex}");

            var asyncTask = new Task(() => _invocationService.InvokeExtensionMethodAsync(extension));
            asyncTask.Start(TaskScheduler.Default);
        }

        private void OnBeforeQueryStatusDynamicItem(object sender, EventArgs args)
        {
            DynamicItemMenuCommand matchedCommand = (DynamicItemMenuCommand)sender;

            matchedCommand.Enabled = !_invocationService.IsRunning;
            matchedCommand.Visible = _matchingMethods.Any();

            var extensionMethodIndex = matchedCommand.DynamicIndex;
            matchedCommand.Text = $"Dynamic {extensionMethodIndex} ({matchedCommand.MatchedCommandId})";

            if (extensionMethodIndex >= 0 && extensionMethodIndex < _maxCount)
            {
                var extensionMethod = _matchingMethods[extensionMethodIndex];
                matchedCommand.Text = extensionMethod.DisplayTitle;
            }

            matchedCommand.Checked = false;
            // Clear the ID because we are done with this item.
            matchedCommand.MatchedCommandId = 0;
        }

        private bool IsValidDynamicItem(int commandId)
        {
            // The match is valid if the command ID is >= the id of our root dynamic start item
            // and the command ID minus the ID of our root dynamic start item
            // is less than or equal to the number of projects in the solution.
            var isValid = (commandId >= (int)_cmdidMyDynamicStartCommand) && ((commandId - (int)_cmdidMyDynamicStartCommand) < _maxCount/*dte2.Solution.Projects.Count*/);

            return isValid;
        }
    }
}
