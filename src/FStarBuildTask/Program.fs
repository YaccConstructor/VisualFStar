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
    let mutable fstarHomePath = ""
    let mutable commandLineArguments = ""

    [<Required>]
    member this.InputFiles
        with get () = items
        and set v = items <- v
            
    [<Required>]
    member this.FStarHomePath
        with get () = fstarHomePath
        and set v = fstarHomePath <- v

    [<Required>]
    member this.CommandLineArguments
        with get () = commandLineArguments
        and set v = commandLineArguments <- v

    interface ITask with
        override this.Execute() =
            
            let args = commandLineArguments + " " + items.[0].ToString()

            let eventArgs = 
                {
                    new CustomBuildEventArgs(message = args , helpKeyword = "", senderName = "FStar") 
                    with member x.Equals(y) = false
                }

            engine.LogCustomEvent(eventArgs)

            try
                FStar.Options.fstar_home_opt := Some @"C:\gsv\projects\YC\FStar\VisualFStar\FStar\"
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



    