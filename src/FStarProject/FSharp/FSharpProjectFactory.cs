using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Project;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace FStarProject.Templates.Projects.FSharpProject
{
    [Guid(FSharpProjectPackageGuids.guidFSharpProjectFactoryString)]
    class FSharpProjectFactory : ProjectFactory
    {
        private FSharpProjectPackage package;

        public FSharpProjectFactory(FSharpProjectPackage package)
            : base(package)
        {   
            this.package = package;
        }

        protected override ProjectNode CreateProject()
        {
            FSharpProjectNode project = new FSharpProjectNode(this.package);

            project.SetSite((IOleServiceProvider)((IServiceProvider)this.package).GetService(typeof(IOleServiceProvider)));
            return project;
        }
    }
}
