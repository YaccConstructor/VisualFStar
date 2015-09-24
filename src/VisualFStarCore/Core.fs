module VisualFStar.Core

open FStar.Parser.ParseIt
open Microsoft.VisualStudio.Package
open FStar.Parser.Parse
open Microsoft.VisualStudio
open Microsoft.VisualStudio.Package
open Microsoft.VisualStudio.TextManager.Interop


type FStarScanner(buffer: IVsTextBuffer) =    
    
    let mutable _buffer = buffer
    let mutable _source = ""
    let mutable getNextToken = fun () -> None

    let tokenize (fileName:string) str =  
        let lexbuf = Microsoft.FSharp.Text.Lexing.LexBuffer<char>.FromString str
        let fs = new System.IO.StreamReader(fileName)
        let lexArgs = FStar.Parser.Lexhelp.mkLexargs ((fun () -> "."), fileName,fs.ReadToEnd())
        let tokenizer = FStar.LexFStar.token lexArgs 
        let data =
            seq{while not lexbuf.IsPastEndOfStream do 
                    match tokenizer lexbuf with
                    | LET _    
                    | IF
                    | MATCH
                    | WITH
                    | IN       -> yield Some (TokenType.Keyword, TokenColor.Keyword, lexbuf)
                    | INT _    -> yield Some (TokenType.Literal, TokenColor.Number, lexbuf) 
                    | CHAR _
                    | STRING _ -> yield Some (TokenType.Literal, TokenColor.String, lexbuf)
                    | COMMENT _ -> yield Some (TokenType.Comment, TokenColor.Comment, lexbuf)
                    | _        -> yield Some (TokenType.Unknown, TokenColor.Text, lexbuf) 
                yield None}
                        
        let enumerator = data.GetEnumerator()
        fun () ->
            let t = enumerator.MoveNext() 
            if t then enumerator.Current else None

    interface IScanner with
        member this.ScanTokenAndProvideInfoAboutIt(tokenInfo, state) =
            match getNextToken() with
            | Some (typ, color, lexbuf:Microsoft.FSharp.Text.Lexing.LexBuffer<char>) -> 
                tokenInfo.Color <- color
                tokenInfo.Type <- typ
                tokenInfo.StartIndex <- lexbuf.StartPos.AbsoluteOffset
                tokenInfo.EndIndex <- lexbuf.EndPos.AbsoluteOffset
                true
            | None -> false

        member this.SetSource(source, offset) =    
            _source <- source.Substring(offset)
            getNextToken <- tokenize @"C:\gsv\projects\YC\FStar\VisualFStar\paket.lock" _source