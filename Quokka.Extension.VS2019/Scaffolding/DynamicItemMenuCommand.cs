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
        private readonly IExtensionInvocationService _invocationService;
        private readonly CommandID _commandId;
        private readonly uint _cmdidMyDynamicStartCommand;
        List<ExtensionMethodInfo> _matchingMethods = new List<ExtensionMethodInfo>();
        private int _maxCount => _matchingMethods.Count;


        public DynamicItemMenuCommand(
            IExtensionInvocationService invocationService,
            CommandID rootId, 
            uint cmdidMyDynamicStartCommand,
            List<ExtensionMethodInfo> matchingMethods,
            EventHandler invokeHandler, 
            EventHandler beforeQueryStatusHandler)
            : base(invokeHandler, null /*changeHandler*/, beforeQueryStatusHandler, rootId)
        {
            _invocationService = invocationService;
            _commandId = rootId;
            _cmdidMyDynamicStartCommand = cmdidMyDynamicStartCommand;
            _matchingMethods = matchingMethods;
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
            var isValid = (commandId >= (int)_cmdidMyDynamicStartCommand) && ((commandId - (int)_cmdidMyDynamicStartCommand) < _maxCount/*dte2.Solution.Projects.Count*/);

            return isValid;
        }

        internal void OnBeforeQueryStatusDynamicItem()
        {
            Enabled = !_invocationService.IsRunning;
            Visible = _matchingMethods.Any();

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
