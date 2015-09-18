module VisualFStar.Core

open FStar.Parser.ParseIt
open Microsoft.VisualStudio.Package
open FStar.Parser.Parse

let tokenize (fileName:string) str =  
    let lexbuf = Microsoft.FSharp.Text.Lexing.LexBuffer<char>.FromString str
    let fs = new System.IO.StreamReader(fileName)
    let lexArgs = FStar.Parser.Lexhelp.mkLexargs ((fun () -> "."), fileName,fs.ReadToEnd())
    let tokenizer = FStar.LexFStar.token lexArgs 
    let data =
        seq{while not lexbuf.IsPastEndOfStream do 
            match tokenizer lexbuf with
            | LET x ->  yield (TokenType.Keyword, TokenColor.Keyword, true)
            | _     -> yield (TokenType.Unknown, TokenColor.Text, true) 
            yield TokenType.Unknown, TokenColor.Text, false}
//        |> fun s -> s.GetEnumerator()
//    fun () ->
//        let r = data.Current
//        data.MoveNext()
//        r
//          
    data
