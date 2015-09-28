using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Project;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace FStarProject.Templates.Projects.FStarProject
{
    [Guid(FStarProjectPackageGuids.guidFStarProjectFactoryString)]
    class FStarProjectFactory : ProjectFactory
    {
        private FStarProjectPackage package;

        public FStarProjectFactory(FStarProjectPackage package)
            : base(package)
        {   
            this.package = package;
        }

        protected override ProjectNode CreateProject()
        {
            FStarProjectNode project = new FStarProjectNode(this.package);

            project.SetSite((IOleServiceProvider)((IServiceProvider)this.package).GetService(typeof(IOleServiceProvider)));
            return project;
        }
    }
}
