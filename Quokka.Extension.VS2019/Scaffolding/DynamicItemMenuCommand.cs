using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;

namespace Quokka.Extension.VS2019
{
    class DynamicItemMenuCommand : OleMenuCommand
    {
        private Predicate<int> matches;
        private readonly CommandID _commandId;

        public DynamicItemMenuCommand(
            CommandID rootId, 
            Predicate<int> matches, 
            EventHandler invokeHandler, 
            EventHandler beforeQueryStatusHandler)
            : base(invokeHandler, null /*changeHandler*/, beforeQueryStatusHandler, rootId)
        {
            _commandId = rootId;
            this.matches = matches ?? throw new ArgumentNullException(nameof(matches));
        }

        public override bool DynamicItemMatch(int cmdId)
        {
            // Call the supplied predicate to test whether the given cmdId is a match.
            // If it is, store the command id in MatchedCommandid
            // for use by any BeforeQueryStatus handlers, and then return that it is a match.
            // Otherwise clear any previously stored matched cmdId and return that it is not a match.
            if (this.matches(cmdId))
            {
                this.MatchedCommandId = cmdId;
                return true;
            }

            this.MatchedCommandId = 0;
            return false;
        }

        public int DynamicIndex => MatchedCommandId == 0 ? 0 : MatchedCommandId - _commandId.ID;
    }
}
