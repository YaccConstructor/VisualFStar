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
        //private string assemblyName;
        private OutputType outputType;
        private string defaultNamespace;
        private string fstarHomePath;
        private string commandLineArguments;

        public GeneralPropertyPage()
        {
            this.Name = "General";
        }

        //[Category("AssemblyName")]
        //[DisplayName("AssemblyName")]
        //[Description("The output file holding assembly metadata.")]
        //public string AssemblyName
        //{
        //    get { return this.assemblyName; }
        //}

        [Category("FStar home path")]
        [DisplayName("Home path")]
        [Description("The path to fstar core.")]
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

        [Category("Application")]
        [DisplayName("OutputType")]
        [Description("The type of application to build.")]
        public OutputType OutputType
        {
            get { return this.outputType; }
            set { this.outputType = value; this.IsDirty = true; }
        }
        [Category("Application")]
        [DisplayName("DefaultNamespace")]
        [Description("Specifies the default namespace for added items.")]
        public string DefaultNamespace
        {
            get { return this.defaultNamespace; }
            set { this.defaultNamespace = value; this.IsDirty = true; }
        }

        protected override void BindProperties()
        {
          //  this.assemblyName = this.ProjectMgr.GetProjectProperty(
          //      "AssemblyName", true);
            this.defaultNamespace = this.ProjectMgr.GetProjectProperty(
                "RootNamespace", false);
            this.fstarHomePath = this.ProjectMgr.GetProjectProperty(
                "FStarHomePath", false);
            this.commandLineArguments = this.ProjectMgr.GetProjectProperty(
                "CommandLineArguments", false);
            string outputType = this.ProjectMgr.GetProjectProperty(
                "OutputType", false);
            this.outputType =
                (OutputType)Enum.Parse(typeof(OutputType), outputType);
        }

        protected override int ApplyChanges()
        {
            //this.ProjectMgr.SetProjectProperty(
            //    "AssemblyName", this.assemblyName);
            this.ProjectMgr.SetProjectProperty(
                "OutputType", this.outputType.ToString());
            this.ProjectMgr.SetProjectProperty(
                "RootNamespace", this.defaultNamespace);
            this.ProjectMgr.SetProjectProperty(
                "FStarHomePath", this.fstarHomePath);
            this.ProjectMgr.SetProjectProperty(
                "CommandLineArguments", this.commandLineArguments);
            this.IsDirty = false;

            return VSConstants.S_OK;
        }
    }
}
