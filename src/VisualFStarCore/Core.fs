module VisualFStar.Core


open Microsoft.VisualStudio.Package
open Microsoft.VisualStudio
open Microsoft.VisualStudio.Package
open Microsoft.VisualStudio.TextManager.Interop

open FStar.Parser.Parse
open FStar.Parser.ParseIt
open FStar.Util


type FStarScanner(buffer: IVsTextBuffer) as this =    
    
    let mutable bufferedC = ""
    let mutable prevState = 0
    
    let mutable _buffer = buffer
    let mutable _source = ""
    let mutable getNextToken = fun _ -> fun _ -> false

    let tokenize (fileName:string) str =  
        let lexbuf = Microsoft.FSharp.Text.Lexing.LexBuffer<char>.FromString str
        //let fs = new System.IO.StreamReader(fileName)
        let lexArgs = FStar.Parser.Lexhelp.mkLexargs ((fun () -> "."), fileName, "")
        let tokenizer = FStar.LexFStar.token lexArgs false
        let data =
            seq{while not lexbuf.IsPastEndOfStream do 
                    yield tokenizer lexbuf |> Some 
                yield None}
                        
        let enumerator = data.GetEnumerator()
        fun (tokenInfo:TokenInfo) state ->
            let token =
                try
                    let t = enumerator.MoveNext() 
                    prevState <- 0
                    if t 
                    then enumerator.Current
                    else None
                with
                | FStar.LexFStar.BlockCommentUnclosed ->
                    //bufferedC <- bufferedC + str
                    //prevState <- 10
                    //(this :>IScanner). // .SetSource(,0)
                    None
                | e -> None                                    
            tokenInfo.Type <- TokenType.Text
            tokenInfo.Color <- TokenColor.Text
            tokenInfo.Trigger <- TokenTriggers.None
            tokenInfo.StartIndex <- lexbuf.StartPos.AbsoluteOffset
            tokenInfo.EndIndex <- lexbuf.EndPos.AbsoluteOffset - 1
            match token with
            | Some x ->
                match x with
                | LET _    
                | IF
                | THEN
                | ELSE
                | FUN
                | MODULE
                | TYPE
                | REC
                | VAL           
                | TRUE
                | FALSE
                | FOR 
                | FORALL                    
                | REQUIRES
                | ENSURES
                | EFFECT
                | KIND
                | OPEN
                | FUNCTION
                | ASSUME 
                | MATCH
                | WITH
                | OF
                | BEGIN
                | END
                | IN       -> 
                    tokenInfo.Type <- TokenType.Keyword                    
                    tokenInfo.Color <- TokenColor.Keyword                    
                    true                
                | CHAR _
                | STRING _ ->                    
                    tokenInfo.Type <- TokenType.Literal
                    tokenInfo.Color <- TokenColor.String                    
                    true
                | COMMENT 
                | LINE_COMMENT ->
                    tokenInfo.Type <- TokenType.Comment
                    tokenInfo.Color <- TokenColor.Comment                    
                    true
                | LBRACE | RBRACE
                | LPAREN | RPAREN
                | LBRACK_BAR | BAR_RBRACK
                | LENS_PAREN_LEFT | LENS_PAREN_RIGHT
                | LBRACK | RBRACK -> 
                    tokenInfo.Trigger <- TokenTriggers.MatchBraces                    
                    true
                | _        -> 
                    tokenInfo.Type <- TokenType.Text
                    tokenInfo.Color <- TokenColor.Text
                    true
            | None -> false

    interface IScanner with
        member this.ScanTokenAndProvideInfoAboutIt(tokenInfo, state) =            
            getNextToken tokenInfo state           

        member this.SetSource(source, offset) = 
            _source <- source.Substring(offset)
            getNextToken <- tokenize @"" _source

type ParseResult =
    | Success of int
    | Error of FStar.Range.range*string

type FStarParser () =
    let parse (req:ParseRequest) =         
        match parse (Inr req.Text) with
        | Inl (Inl ast) ->                                
          //Desugar.desugar_file env ast
          Success 1

        | Inl (Inr _) ->
          //Util.fprint1 "%s: Expected a module\n" fn;
          //exit 1
          Success 0
        | Inr (msg, r) ->
            let mutable span = new TextSpan()

            span.iStartIndex <- FStar.Range.start_of_range r |> FStar.Range.col_of_pos
            span.iStartLine  <- FStar.Range.start_of_range r |> FStar.Range.line_of_pos

            span.iEndIndex <- FStar.Range.end_of_range r |> FStar.Range.col_of_pos
            span.iEndLine  <- FStar.Range.end_of_range r |> FStar.Range.line_of_pos

            req.Sink.AddError(req.FileName,msg,span,Severity.Error)
            Error(r, msg)

    member this.Parse (req:ParseRequest) =        
        parse req