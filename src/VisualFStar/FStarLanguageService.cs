using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.OLE.Interop;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;

namespace VisualFStar
{
    
    public class FStarLanguageService : LanguageService
    {
        private LanguagePreferences m_preferences;
        private Core.FStarScanner  m_scanner;
        private ColorableItem[] m_colorableItems;

        public FStarLanguageService() : base()
        {
            //m_colorableItems = new ColorableItem[] {
            //    new ColorableItem("TestLanguage – sa",
            //                      "asd",
            //                      COLORINDEX.CI_MAROON,
            //                      COLORINDEX.CI_SYSPLAINTEXT_BK,
            //                      System.Drawing.Color.FromArgb(192,32,32),
            //                      System.Drawing.Color.Empty,
            //                      FONTFLAGS.FF_BOLD),
            //new ColorableItem("TestLanguage – Keyword",
            //                      "Keyword",
            //                      COLORINDEX.CI_MAROON,
            //                      COLORINDEX.CI_SYSPLAINTEXT_BK,
            //                      System.Drawing.Color.FromArgb(192, 32, 32),
            //                      System.Drawing.Color.DeepSkyBlue,
            //                      FONTFLAGS.FF_BOLD)};

}

        public override LanguagePreferences GetLanguagePreferences()
        {
            if (m_preferences == null)
            {
                m_preferences = new LanguagePreferences(this.Site,
                                                        typeof(FStarLanguageService).GUID,
                                                        this.Name);
                m_preferences.Init();
            }
            return m_preferences;
        }
        
        public override IScanner GetScanner(IVsTextLines buffer)
        {
            if (m_scanner == null)
            {
                m_scanner = new Core.FStarScanner(buffer);
            }
            return m_scanner;
        }

        public override AuthoringScope ParseSource(ParseRequest req)
        {
            return new TestAuthoringScope();
            //req.Sink.AddError
        }

        public override string Name
        {
            get { return "FStar"; }
        }

        public override string GetFormatFilterList()
        {
            return "*.fst";
        }
    }
}

internal class TestAuthoringScope : AuthoringScope
{
    public override string GetDataTipText(int line, int col, out TextSpan span)
    {
        span = new TextSpan();
        return null;
    }

    public override Declarations GetDeclarations(IVsTextView view,
                                                 int line,
                                                 int col,
                                                 TokenInfo info,
                                                 ParseReason reason)
    {
        return null;
    }

    public override string Goto(VSConstants.VSStd97CmdID cmd, IVsTextView textView, int line, int col, out TextSpan span)
    {
        span = new TextSpan();
        return null;
    }

    public override Methods GetMethods(int line, int col, string name)
    {
        return null;
    }
}