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
        private TestScanner m_scanner;

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
                m_scanner = new TestScanner(buffer);
            }
            return m_scanner;
        }

        public override AuthoringScope ParseSource(ParseRequest req)
        {
            return new TestAuthoringScope();
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

internal class TestScanner : IScanner
{
    private IVsTextBuffer m_buffer;
    string m_source;
    private IEnumerator<Tuple<TokenType, TokenColor, bool>> tokens;

    public TestScanner(IVsTextBuffer buffer)
    {
        m_buffer = buffer;
       // VisualFStar.Core.tokenize(@"C:\gsv\projects\YC\FStar\VisualFStar\paket.lock", "");
    }

    bool IScanner.ScanTokenAndProvideInfoAboutIt(TokenInfo tokenInfo, ref int state)
    {
        //tokenInfo.Type = TokenType.Unknown;
        //tokenInfo.Color = TokenColor.Text;
        //return true;

        tokens.MoveNext();
        var r = tokens.Current;        
        tokenInfo.Type = r.Item1;
        tokenInfo.Color = r.Item2;
        return r.Item3;
    }

    void IScanner.SetSource(string source, int offset)
    {        
        m_source = source.Substring(offset);
        tokens = VisualFStar.Core.tokenize(@"C:\gsv\projects\YC\FStar\VisualFStar\paket.lock", m_source).GetEnumerator();
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