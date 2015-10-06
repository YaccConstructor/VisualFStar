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
            with
            | e -> printfn "Error: %A" e.Message
            true
        
        override this.HostObject
            with get () = host
            and set v = host <- v

        override this.BuildEngine
            with get () = engine
            and set v =  engine <- v



    