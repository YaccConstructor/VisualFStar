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
    public class FSharpProjectNode : ProjectNode
    {
        private FSharpProjectPackage package;

        private static ImageList imageList;

        internal static int imageIndex;
        public override int ImageIndex
        {
            get { return imageIndex; }
        }

        static FSharpProjectNode()
        {
            imageList = Utilities.GetImageList(typeof(FSharpProjectNode).Assembly.GetManifestResourceStream("FStarProject.Resources.FStarProjectNode.bmp"));
            int a = imageList.ImageSize.Height;
            int b = imageList.ImageSize.Width;
        }

        public FSharpProjectNode(FSharpProjectPackage package)
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
            get { return FSharpProjectPackageGuids.guidFSharpProjectFactory; }
        }
        public override string ProjectType
        {
            get { return "FSharp"; }
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
