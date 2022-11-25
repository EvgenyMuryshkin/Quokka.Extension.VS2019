using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using Quokka.Extension.Interface;
using Quokka.Extension.Scaffolding;
using Quokka.Extension.Interop;
using System.Collections.Generic;
using System.Linq;
using Task = System.Threading.Tasks.Task;

namespace Quokka.Extension.VS2022
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

            _ens.OnExtensionsReloaded += (s, a) =>
            {
                Reload();
            };
        }

        void Reload()
        {
            OleMenuCommandService commandService = _serviceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService == null)
            {
                _logger.WriteLine("OleMenuCommandService was not resolved");
                return;
            }

            // Add the DynamicItemMenuCommand for the expansion of the root item into N items at run time.
            CommandID dynamicItemRootId = new CommandID(_commandsSetId, (int)_cmdidMyDynamicStartCommand);

            var matchingMethods = _extensionsCacheService.ExtensionsForIcon(_icon);
            var existing = commandService.FindCommand(dynamicItemRootId) as DynamicItemMenuCommand;
            if (existing != null)
            {
                existing.Update(matchingMethods);
            }
            else
            {
                DynamicItemMenuCommand dynamicMenuCommand = new DynamicItemMenuCommand(
                    _extensionsCacheService,
                    _invocationService,
                    dynamicItemRootId,
                    _cmdidMyDynamicStartCommand,
                    matchingMethods
                    )
                {
                    Visible = false,
                    Enabled = false
                };
                commandService.AddCommand(dynamicMenuCommand);
            }
        }

        public Task InitializeAsync()
        {
            Reload();
            return Task.CompletedTask;
        }
    }
}
