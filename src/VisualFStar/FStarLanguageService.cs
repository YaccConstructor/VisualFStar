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
    class FStarSource : Source
    {
        public FStarSource(LanguageService service, IVsTextLines textLines, Colorizer colorizer) : base(service, textLines, colorizer)
        {

        }

        public override void OnIdle(bool isPeriodic)
        {
            this.BeginParse();
            base.OnIdle(isPeriodic);
        }
        
        public override CommentInfo GetCommentFormat()
        {
            CommentInfo info = new CommentInfo();
            info.LineStart = "//";
            info.BlockStart = "(*";
            info.BlockEnd = "*)";
            info.UseLineComments = true;
            return info;
        }
    }

    public class FStarLanguageService : LanguageService
    {
        private LanguagePreferences m_preferences;
        private Core.FStarScanner  m_scanner;
        private ColorableItem[] m_colorableItems;

        public FStarLanguageService() : base()
        {
            
        }

        public override Source CreateSource(IVsTextLines buffer)
        {
            return new FStarSource(this,buffer,this.GetColorizer(buffer));
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
            Core.FStarParser parser = new Core.FStarParser();
            var result = new TestAuthoringScope();
            var tokens = new List<TokenInfo>();
            if (req.Sink.BraceMatching)
            {
                var scanner = GetScanner(null);
                scanner.SetSource(req.Text, 0);
                var line = 0;
                var col = 0;
                req.View.GetCaretPos(out line, out col);
                (scanner as VisualFStar.Core.FStarScanner).MatchPair(req.Sink, req.Text, line, col);
                
            }
            if (ParseReason.Check == req.Reason)
            {
                Console.WriteLine("!!!!");
            }
            parser.Parse(req);                        
            return result;
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
        var s = new TextSpan();
        s.iStartIndex = col;
        s.iStartLine = line;
        s.iEndIndex = col+1;
        s.iEndLine = line;
        span = s; 
        return "uaaaa!!!";
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