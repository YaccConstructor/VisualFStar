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
        seq{while not lexbuf.IsPastEndOfStream do 
                yield tokenizer lexbuf |> Some 
            yield None}
        , lexbuf
                        
    let getTokenInfo (fileName:string) str =
        let data,lexbuf = tokenize fileName str
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
                | FUNCTION
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
                | EXISTS
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
            getNextToken <- getTokenInfo @"" _source
            
    member this.MatchPair (sink:AuthoringSink) str line col =
        let data,lexbuf = tokenize @"" str
        let enumerator = data.GetEnumerator()
        let stackBrace = new System.Collections.Generic.Stack<_>()
        let stackParen = new System.Collections.Generic.Stack<_>()
        let stackBrack = new System.Collections.Generic.Stack<_>()
        let stackBarBrack = new System.Collections.Generic.Stack<_>()
        let stackLens = new System.Collections.Generic.Stack<_>()
        let stop = ref false 
        
        let createSpan (pairPosition : Microsoft.FSharp.Text.Lexing.Position * Microsoft.FSharp.Text.Lexing.Position) =
            let curBrStart = fst pairPosition
            let curBrEnd = snd pairPosition
            if line = lexbuf.StartPos.Line
            then 
                if col = lexbuf.StartPos.Column + 1 || col = curBrStart.Column + 1
                then
                    stop := true
                    let mutable span1 = new TextSpan()
                    let mutable span2 = new TextSpan()                                
                    span1.iStartLine <- curBrStart.Line
                    span1.iStartIndex <- curBrStart.Column
                    span1.iEndLine <- curBrStart.Line
                    span1.iEndIndex <- curBrEnd.Column                                
                    span2.iStartLine <- lexbuf.StartPos.Line
                    span2.iStartIndex <- lexbuf.StartPos.Column
                    span2.iEndLine <- lexbuf.EndPos.Line
                    span2.iEndIndex <- lexbuf.EndPos.Column
                    sink.MatchPair (span1,span2,1)
                              
        while not !stop do
            let t = enumerator.MoveNext()
            if not t
            then stop := true 
            else 
                let token = enumerator.Current
                match token with
                | None -> ()
                | Some token ->
                    match token with 
                    
                    | LBRACE -> 
                        stackBrace.Push(lexbuf.StartPos, lexbuf.EndPos)
                    | RBRACE -> 
                        createSpan <| stackBrace.Pop()
                    | LPAREN -> 
                        stackParen.Push((lexbuf.StartPos), lexbuf.EndPos)
                    | RPAREN ->
                        createSpan <| stackParen.Pop()
                    | LBRACK_BAR -> 
                        stackBarBrack.Push(lexbuf.StartPos, lexbuf.EndPos)
                    | BAR_RBRACK ->
                        createSpan <| stackBarBrack.Pop()
                    | LENS_PAREN_LEFT ->
                        stackLens.Push(lexbuf.StartPos, lexbuf.EndPos)
                    | LENS_PAREN_RIGHT ->
                        createSpan <| stackLens.Pop()
                    | LBRACK -> 
                        stackBrack.Push(lexbuf.StartPos, lexbuf.EndPos)
                    | RBRACK -> 
                        createSpan <| stackBrack.Pop()  
                    | LPAREN_RPAREN ->
                        createSpan <| (lexbuf.StartPos, lexbuf.EndPos)                   
                    | _ -> ()
      


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