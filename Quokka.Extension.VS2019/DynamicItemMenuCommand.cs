using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;

using EnvDTE;
using EnvDTE80;

namespace Quokka.Extension.VS2019
{
    class DynamicMenu
    {
        public const string guidDynamicMenuPackageCmdSet = "4bb7016f-f3ce-4305-91c2-2493253e2325";  // get the GUID from the .vsct file
        public const uint cmdidMyCommand = 0x2000;

        private DTE2 dte2;
        private int rootItemId = 0;

        private readonly AsyncPackage package;
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        internal DynamicMenu(AsyncPackage package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            this.package = package;

            OleMenuCommandService commandService = (this.ServiceProvider.GetServiceAsync(typeof(IMenuCommandService))).Result as OleMenuCommandService;
            if (commandService != null)
            {
                // Add the DynamicItemMenuCommand for the expansion of the root item into N items at run time.
                CommandID dynamicItemRootId = new CommandID(new Guid(guidDynamicMenuPackageCmdSet), (int)cmdidMyCommand);
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

            dte2 = (DTE2)(this.ServiceProvider.GetServiceAsync(typeof(DTE)).Result);
        }

        static int maxCount = 4;
        private void OnInvokedDynamicItem(object sender, EventArgs args)
        {
            maxCount++;
            return;

            DynamicItemMenuCommand invokedCommand = (DynamicItemMenuCommand)sender;
            // If the command is already checked, we don't need to do anything
            if (invokedCommand.Checked)
                return;

            // Find the project that corresponds to the command text and set it as the startup project
            var projects = dte2.Solution.Projects;
            foreach (Project proj in projects)
            {
                if (invokedCommand.Text.Equals(proj.Name))
                {
                    dte2.Solution.SolutionBuild.StartupProjects = proj.FullName;
                    return;
                }
            }
        }

        private void OnBeforeQueryStatusDynamicItem(object sender, EventArgs args)
        {
            DynamicItemMenuCommand matchedCommand = (DynamicItemMenuCommand)sender;
            matchedCommand.Enabled = true;
            matchedCommand.Visible = true;

            // Find out whether the command ID is 0, which is the ID of the root item.
            // If it is the root item, it matches the constructed DynamicItemMenuCommand,
            // and IsValidDynamicItem won't be called.
            bool isRootItem = (matchedCommand.MatchedCommandId == 0);

            // The index is set to 1 rather than 0 because the Solution.Projects collection is 1-based.
            int indexForDisplay = (isRootItem ? 1 : (matchedCommand.MatchedCommandId - (int)cmdidMyCommand) + 1);

            matchedCommand.Text = "Dynamic " +  indexForDisplay.ToString(); //dte2.Solution.Projects.Item(indexForDisplay).Name;
            /*
            Array startupProjects = (Array)dte2.Solution.SolutionBuild.StartupProjects;
            string startupProject = System.IO.Path.GetFileNameWithoutExtension((string)startupProjects.GetValue(0));

            // Check the command if it isn't checked already selected
            matchedCommand.Checked = (matchedCommand.Text == startupProject);
            */

            matchedCommand.Checked = false;
            // Clear the ID because we are done with this item.
            matchedCommand.MatchedCommandId = 0;
        }

        private bool IsValidDynamicItem(int commandId)
        {
            // The match is valid if the command ID is >= the id of our root dynamic start item
            // and the command ID minus the ID of our root dynamic start item
            // is less than or equal to the number of projects in the solution.
            return (commandId >= (int)cmdidMyCommand) && ((commandId - (int)cmdidMyCommand) < maxCount/*dte2.Solution.Projects.Count*/);
        }
    }

    class DynamicItemMenuCommand : OleMenuCommand
    {
        private Predicate<int> matches;

        public DynamicItemMenuCommand(
            CommandID rootId, 
            Predicate<int> matches, 
            EventHandler invokeHandler, 
            EventHandler beforeQueryStatusHandler)
            : base(invokeHandler, null /*changeHandler*/, beforeQueryStatusHandler, rootId)
        {
            if (matches == null)
            {
                throw new ArgumentNullException("matches");
            }

            this.matches = matches;
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
    }
}
