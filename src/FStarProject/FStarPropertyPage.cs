using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;

namespace FStarProject
{
    class FStarPropertyPage : Form, Microsoft.VisualStudio.OLE.Interop.IPropertyPage
    {
        public void Activate(IntPtr hWndParent, RECT[] pRect, int bModal)
        {
            throw new NotImplementedException();
        }

        public int Apply()
        {
            throw new NotImplementedException();
        }

        //Summary: Return a stucture describing your property page.
        public void GetPageInfo(Microsoft.VisualStudio.OLE.Interop.PROPPAGEINFO[] pPageInfo)
            {
                PROPPAGEINFO info = new PROPPAGEINFO();
                info.cb = (uint)Marshal.SizeOf(typeof(PROPPAGEINFO));
                info.dwHelpContext = 0;
                info.pszDocString = null;
                info.pszHelpFile = null;
                info.pszTitle = "Deployment";  //Assign tab name
                info.SIZE.cx = this.Size.Width;
                info.SIZE.cy = this.Size.Height;
                if (pPageInfo != null && pPageInfo.Length > 0)
                    pPageInfo[0] = info;
            }

        public void Help(string pszHelpDir)
        {
            throw new NotImplementedException();
        }

        public int IsPageDirty()
        {
            throw new NotImplementedException();
        }

        public void SetObjects(uint cObjects, object[] ppunk)
        {
            throw new NotImplementedException();
        }

        public void SetPageSite(IPropertyPageSite pPageSite)
        {
            throw new NotImplementedException();
        }

        public void Show(uint nCmdShow)
        {
            throw new NotImplementedException();
        }

        public int TranslateAccelerator(MSG[] pMsg)
        {
            throw new NotImplementedException();
        }

        void IPropertyPage.Deactivate()
        {
            throw new NotImplementedException();
        }

        void IPropertyPage.Move(RECT[] pRect)
        {
            throw new NotImplementedException();
        }
    }
}
