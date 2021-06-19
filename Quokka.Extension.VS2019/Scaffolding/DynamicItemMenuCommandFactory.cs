using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using Quokka.Extension.Interface;
using Quokka.Extension.Scaffolding;
using Quokka.Extension.Interop;
using System.Collections.Generic;
using System.Linq;
using Task = System.Threading.Tasks.Task;

namespace Quokka.Extension.VS2019
{
    public class DynamicItemMenuCommandFactory : ExtensionService, IExtensionPart
    {
        private readonly IExtensionsCacheService _extensionsCacheService;
        private readonly IExtensionInvocationService _invocationService;

        private readonly Guid _commandsSetId;
        private readonly int _cmdidMyDynamicStartCommand;
        private readonly ExtensionMethodIcon _icon;

        public DynamicItemMenuCommandFactory(
            ExtensionDeps deps,
            IExtensionInvocationService invocationService,
            IExtensionsCacheService extensionsCacheService,
            Guid commandsSetId,
            int cmdidMyDynamicStartCommand, 
            ExtensionMethodIcon icon
            ) : base(deps)
        {
            _invocationService = invocationService;
            _extensionsCacheService = extensionsCacheService;
            _commandsSetId = commandsSetId;
            _cmdidMyDynamicStartCommand = cmdidMyDynamicStartCommand;
            _icon = icon;
        }

        public Task InitializeAsync()
        {
            OleMenuCommandService commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService == null)
            {
                Logger.WriteLine("OleMenuCommandService was not resolved");
                return Task.CompletedTask;
            }

            // Add the DynamicItemMenuCommand for the expansion of the root item into N items at run time.
            CommandID dynamicItemRootId = new CommandID(_commandsSetId, (int)_cmdidMyDynamicStartCommand);

            var existing = commandService.FindCommand(dynamicItemRootId);
            if (existing != null)
                commandService.RemoveCommand(existing);

            var matchingMethods = _extensionsCacheService.ExtensionsForIcon(_icon);
            if (matchingMethods.Any())
            {
                DynamicItemMenuCommand dynamicMenuCommand = new DynamicItemMenuCommand(
                    _invocationService,
                    dynamicItemRootId,
                    _cmdidMyDynamicStartCommand,
                    matchingMethods,
                    OnInvokedDynamicItem,
                    OnBeforeQueryStatusDynamicItem
                    )
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
            invokedCommand.Invoke();
        }

        private void OnBeforeQueryStatusDynamicItem(object sender, EventArgs args)
        {
            DynamicItemMenuCommand matchedCommand = (DynamicItemMenuCommand)sender;
            matchedCommand.OnBeforeQueryStatusDynamicItem();
        }
    }
}
