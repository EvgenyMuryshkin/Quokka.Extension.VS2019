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
    public class DynamicIconsCommandFactory : ExtensionService, IExtensionPart
    {
        private readonly IExtensionsCacheService _extensionsCacheService;
        private readonly IExtensionInvocationService _invocationService;

        public DynamicIconsCommandFactory(
            ExtensionDeps deps,
            IExtensionInvocationService invocationService,
            IExtensionsCacheService extensionsCacheService
            ) : base(deps)
        {
            _invocationService = invocationService;
            _extensionsCacheService = extensionsCacheService;
        }

        public Task InitializeAsync()
        {
            OleMenuCommandService commandService = _serviceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService == null)
            {
                _logger.WriteLine("OleMenuCommandService was not resolved");
                return Task.CompletedTask;
            }

            var id = 1;
            foreach (var iconType in ExtensionCatalogue.IconTypes)
            {
                var names = Enum.GetNames(iconType);
                var values = Enum.GetValues(iconType);

                for (var idx = 0; idx < names.Length; idx++)
                {
                    var cmdidMyDynamicStartCommand = id * 100;

                    var value = (int)values.GetValue(idx);
                    var icon = new ExtensionMethodIcon(iconType, value);

                    CommandID dynamicItemRootId = new CommandID(guidDynamicCommandsSet.SetId, cmdidMyDynamicStartCommand);

                    var existing = commandService.FindCommand(dynamicItemRootId);
                    if (existing != null)
                        commandService.RemoveCommand(existing);

                    var matchingMethods = _extensionsCacheService.ExtensionsForIcon(icon);
                    if (matchingMethods.Any())
                    {
                        DynamicItemMenuCommand dynamicMenuCommand = new DynamicItemMenuCommand(
                            _extensionsCacheService,
                            _invocationService,
                            dynamicItemRootId,
                            cmdidMyDynamicStartCommand,
                            matchingMethods
                            )
                        {
                            Visible = false
                        };
                        commandService.AddCommand(dynamicMenuCommand);
                    }

                    id++;
                }
            }

            return Task.CompletedTask;
        }
    }
}
