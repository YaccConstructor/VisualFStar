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
                    | LET x ->  yield Some (TokenType.Keyword, TokenColor.Keyword)
                    | _     -> yield Some (TokenType.Unknown, TokenColor.Text) 
                yield None}
        //let arr = data |> List.ofSeq
        //arr  |> printfn "%A"
        let enumerator = data.GetEnumerator()
        fun () ->
            let t = enumerator.MoveNext() 
            if t then enumerator.Current else None

    interface IScanner with
        member this.ScanTokenAndProvideInfoAboutIt(tokenInfo, state) =
            match getNextToken() with
            | Some (typ, color) -> 
                tokenInfo.Color <- color
                tokenInfo.Type <- typ
                true
            | None -> false

        member this.SetSource(source, offset) =    
            _source <- source.Substring(offset)
            getNextToken <- tokenize @"C:\Users\User\VisualFStar\paket.lock" _source