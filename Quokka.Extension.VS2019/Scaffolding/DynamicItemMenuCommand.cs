using Microsoft.VisualStudio.Shell;
using Quokka.Extension.Interface;
using Quokka.Extension.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Quokka.Extension.VS2019
{
    class DynamicItemMenuCommand : OleMenuCommand
    {
        private readonly IExtensionsCacheService _ecs;
        private readonly IExtensionInvocationService _invocationService;
        private readonly CommandID _commandId;
        private readonly int _cmdidMyDynamicStartCommand;
        List<ExtensionMethodInfo> _matchingMethods = new List<ExtensionMethodInfo>();
        private int _maxCount => _matchingMethods.Count;
        private bool hasExtensions;

        public DynamicItemMenuCommand(
            IExtensionsCacheService ecs,
            IExtensionInvocationService invocationService,
            CommandID rootId, 
            int cmdidMyDynamicStartCommand,
            List<ExtensionMethodInfo> matchingMethods)
            : base(OnInvokedDynamicItem, null /*changeHandler*/, OnBeforeQueryStatusDynamicItem, rootId)
        {
            _ecs = ecs;
            _invocationService = invocationService;
            _commandId = rootId;
            _cmdidMyDynamicStartCommand = cmdidMyDynamicStartCommand;
            _matchingMethods = matchingMethods;
        }

        static void OnInvokedDynamicItem(object sender, EventArgs args)
        {
            DynamicItemMenuCommand invokedCommand = (DynamicItemMenuCommand)sender;
            invokedCommand.Invoke();
        }

        static void OnBeforeQueryStatusDynamicItem(object sender, EventArgs args)
        {
            DynamicItemMenuCommand matchedCommand = (DynamicItemMenuCommand)sender;
            matchedCommand.OnBeforeQueryStatusDynamicItem();
        }

        public void Update(List<ExtensionMethodInfo> matchingMethods)
        {
            hasExtensions = matchingMethods.Any();

            if (matchingMethods.Any())
                _matchingMethods = matchingMethods;

            Enabled = !_invocationService.IsRunning && hasExtensions;
            Visible = hasExtensions;
        }

        public override void Invoke()
        {
            var extension = DynamicExtension;

            if (extension != null)
            {
                var asyncTask = new Task(() => _invocationService.InvokeExtensionMethodAsync(extension));
                asyncTask.Start(TaskScheduler.Default);
            }
        }

        public override bool DynamicItemMatch(int cmdId)
        {
            // Call the supplied predicate to test whether the given cmdId is a match.
            // If it is, store the command id in MatchedCommandid
            // for use by any BeforeQueryStatus handlers, and then return that it is a match.
            // Otherwise clear any previously stored matched cmdId and return that it is not a match.
            if (IsValidDynamicItem(cmdId))
            {
                this.MatchedCommandId = cmdId;
                return true;
            }

            this.MatchedCommandId = 0;
            return false;
        }

        public int DynamicIndex => MatchedCommandId == 0 ? 0 : MatchedCommandId - _commandId.ID;
        public ExtensionMethodInfo DynamicExtension
        {
            get
            {
                var index = DynamicIndex;
                if (index < 0 || index >= _matchingMethods.Count)
                    return null;

                return _matchingMethods[index];
            }
        }

        private bool IsValidDynamicItem(int commandId)
        {
            // The match is valid if the command ID is >= the id of our root dynamic start item
            // and the command ID minus the ID of our root dynamic start item
            // is less than or equal to the number of projects in the solution.
            var isValid = (commandId >= _cmdidMyDynamicStartCommand) && ((commandId - _cmdidMyDynamicStartCommand) < _maxCount/*dte2.Solution.Projects.Count*/);

            return isValid;
        }

        internal void OnBeforeQueryStatusDynamicItem()
        {
            Enabled = !_invocationService.IsRunning && hasExtensions;
            Visible = hasExtensions;

            var extensionMethodIndex = DynamicIndex;
            Text = $"Dynamic {extensionMethodIndex} ({MatchedCommandId})";

            if (extensionMethodIndex >= 0 && extensionMethodIndex < _maxCount)
            {
                var extensionMethod = _matchingMethods[extensionMethodIndex];
                Text = extensionMethod.DisplayTitle;
            }

            // TOOD: what is next code supposed to do?
            Checked = false;
            // Clear the ID because we are done with this item.
            MatchedCommandId = 0;
        }
    }
}
