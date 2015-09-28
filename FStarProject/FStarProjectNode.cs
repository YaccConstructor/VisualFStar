using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Project;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace FStarProject
{
    public class FStarProjectNode : ProjectNode
    {
        private FStarProjectPackage package;

        private static ImageList imageList;

        internal static int imageIndex;
        public override int ImageIndex
        {
            get { return imageIndex; }
        }

        static FStarProjectNode()
        {
            imageList = Utilities.GetImageList(typeof(FStarProjectNode).Assembly.GetManifestResourceStream("FStarProject.Resources.FStarProjectNode.bmp"));
            int a = imageList.ImageSize.Height;
            int b = imageList.ImageSize.Width;
        }

        public FStarProjectNode(FStarProjectPackage package)
        {
            this.package = package;

            imageIndex = this.ImageHandler.ImageList.Images.Count;

            foreach (Image img in imageList.Images)
            {
                this.ImageHandler.AddImage(img);
            }
        }

        public override Guid ProjectGuid
        {
            get { return FStarProjectPackageGuids.guidFStarProjectFactory; }
        }
        public override string ProjectType
        {
            get { return "FStarProjectType"; }
        }

        protected override Guid[] GetConfigurationIndependentPropertyPages()
        {
            Guid[] result = new Guid[1];
            result[0] = typeof(GeneralPropertyPage).GUID;
            return result;
        }
        protected override Guid[] GetPriorityProjectDesignerPages()
        {
            Guid[] result = new Guid[1];
            result[0] = typeof(GeneralPropertyPage).GUID;
            return result;
        }
    }
}
