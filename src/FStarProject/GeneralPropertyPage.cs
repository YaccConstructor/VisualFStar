using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Project;
using System.ComponentModel;

namespace FStarProject
{
    [ComVisible(true)]
    [Guid("6BC7046B-B110-40d8-9F23-34263D8D2936")]
    public class GeneralPropertyPage : SettingsPage
    {
        private string fstarHomePath;
        private string commandLineArguments;

        public GeneralPropertyPage()
        {
            this.Name = "General";
        }

        [Category("FStar home path")]
        [DisplayName("Home path")]
        [Description("The path to FStar core.")]
        public string FStarHomePath
        {
            get { return this.fstarHomePath; }
            set { this.fstarHomePath = value; this.IsDirty = true; }
        }

        [Category("Command line arguments")]
        [DisplayName("Arguments")]
        [Description("Arguments for FStar compiler.")]
        public string CommandLineArguments
        {
            get { return this.commandLineArguments; }
            set { this.commandLineArguments = value; this.IsDirty = true; }
        }
        
        protected override void BindProperties()
        {
            this.fstarHomePath = this.ProjectMgr.GetProjectProperty(
                "FStarHomePath", false);
            this.commandLineArguments = this.ProjectMgr.GetProjectProperty(
                "CommandLineArguments", false);
        }

        protected override int ApplyChanges()
        {
            this.ProjectMgr.SetProjectProperty(
                "FStarHomePath", this.fstarHomePath);
            this.ProjectMgr.SetProjectProperty(
                "CommandLineArguments", this.commandLineArguments);
            this.IsDirty = false;

            return VSConstants.S_OK;
        }
    }
}
