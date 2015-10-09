// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help
namespace FStarBuild

open Microsoft.Build.Framework
open System.IO
open Microsoft.FSharp.Text
open FStar

[<Class>] 
type FStar() as this =
    let mutable engine = Unchecked.defaultof<IBuildEngine>
    let mutable host = Unchecked.defaultof<ITaskHost> 
    let mutable items = Array.empty<ITaskItem>

    [<Required>]
    member this.InputFiles
        with get () = items
        and set v = items <- v
            
    interface ITask with
        override this.Execute() =
            
            let args = items.[0].ToString()
            try
                FStar.FStar.goInternal args
                true
            with
            | e -> 
                let error = 
                    new BuildErrorEventArgs(
                        subcategory = "error",
                        code = "",
                        file = args,
                        lineNumber = 1,
                        columnNumber = 1,
                        endLineNumber = 1,
                        endColumnNumber = 10,
                        message = e.Message,
                        helpKeyword = "",
                        senderName = "FStar")
                printfn "C:\sourcefile.cpp(134) : error C2143: syntax error : missing ';' before '}' %A" e.Message
                engine.LogErrorEvent(error)
                false            
        
        override this.HostObject
            with get () = host
            and set v = host <- v

        override this.BuildEngine
            with get () = engine
            and set v =  engine <- v



    