// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help
namespace FStarBuild

open Microsoft.Build.Framework
open Microsoft.Build.Utilities
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
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

            let output msg =        
                // Get the output window
                let outputWindow = Package.GetGlobalService(typeof<SVsOutputWindow>) :?> IVsOutputWindow
 
                // Ensure that the desired pane is visible
                let paneGuid = Microsoft.VisualStudio.VSConstants.OutputWindowPaneGuid.BuildOutputPane_guid
                let i,pane = outputWindow.GetPane(ref paneGuid)
 
                // Output the message
                pane.OutputString(msg) |> ignore
                                
            "Verification started."
            |> output 

            "FStar " + args
            |> output 

            try
                FStar.Options.fstar_home_opt := Some @"C:\gsv\projects\YC\FStar\VisualFStar\FStar\"
                FStar.FStar.goInternal args
                true
            with
            | e -> 
                let error = 
                    new BuildErrorEventArgs(
                        subcategory = "Error",
                        code = "",
                        file = args,
                        lineNumber = 1,
                        columnNumber = 1,
                        endLineNumber = 1,
                        endColumnNumber = 10,
                        message = e.Message,
                        helpKeyword = "",
                        senderName = "FStar")
                engine.LogErrorEvent(error)
                false            
        
        override this.HostObject
            with get () = host
            and set v = host <- v

        override this.BuildEngine
            with get () = engine
            and set v =  engine <- v
