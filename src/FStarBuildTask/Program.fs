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
    let mutable fstarHomePath = ""
    let mutable commandLineArguments = ""
    let mutable outputFile = ""

    [<Required>]
    member this.InputFiles
        with get () = items
        and set v = items <- v
            
    [<Required>]
    member this.FStarHomePath
        with get () = fstarHomePath
        and set v = fstarHomePath <- v
    
    member this.CommandLineArguments
        with get () = commandLineArguments
        and set v = commandLineArguments <- v

    [<Output>]
    member this.OutputFile
        with get ()  = outputFile
        and  set (x) = outputFile <- x

    [<Required>]
    member val FStarTargetsPath = "" with get, set

    interface ITask with
        override this.Execute() =
            let fStarHome = "\"" + this.FStarHomePath.TrimEnd('\\') + "\""
            let args = 
                "\"" + items.[0].GetMetadata "FullPath" + "\""
                + " --fstar_home " + fStarHome
                + " " + if System.String.IsNullOrEmpty(commandLineArguments) then "" else commandLineArguments

            let output msg =
                // Get the output window
                let outputWindow = Package.GetGlobalService(typeof<SVsOutputWindow>) :?> IVsOutputWindow
                lock 
                    fStarHome 
                    (fun () -> 
                // Ensure that the desired pane is visible
                    let paneGuid = Microsoft.VisualStudio.VSConstants.OutputWindowPaneGuid.BuildOutputPane_guid
                    let i,pane = outputWindow.GetPane(ref paneGuid)                    
                    // Output the message
                    try
                        let regexp = System.Text.RegularExpressions.Regex("(.*)\(([0-9]+),([0-9]+)-([0-9]+),([0-9]+)\)(.*)")
                        let m = regexp.Match(msg)
                        if m.Success
                        then
                            let file, lnFrom, colFrom, lnTo, colTo, body =
                        
                                m.Groups.[1].Captures.[0].Value
                                , int m.Groups.[2].Captures.[0].Value
                                , int m.Groups.[3].Captures.[0].Value
                                , int m.Groups.[4].Captures.[0].Value
                                , int m.Groups.[5].Captures.[0].Value
                                , m.Groups.[6].Captures.[0].Value.Trim().Trim(':')

                            let error = 
                                new BuildErrorEventArgs(
                                    subcategory = "Error",
                                    code = "",
                                    file = file,
                                    lineNumber = lnFrom,
                                    columnNumber = colFrom,
                                    endLineNumber = lnTo,
                                    endColumnNumber = colTo,
                                    message = body,
                                    helpKeyword = "",
                                    senderName = "FStar")
                            engine.LogErrorEvent(error)
                        else pane.OutputString("\n> " + msg + "\n") |> ignore

                    with
                    | _ -> ())
                                
            "Verification started."
            |> output 

            "FStar " + args
            |> output 

            try
                let p = new System.Diagnostics.Process()                
                p.StartInfo.FileName <- System.IO.Path.Combine(System.IO.Path.GetDirectoryName this.FStarTargetsPath, "FStar.exe")
                p.StartInfo.Arguments <- args                
                p.StartInfo.RedirectStandardOutput <- true
                p.StartInfo.RedirectStandardError <- true

                //* Set your output and error (asynchronous) handlers
                p.OutputDataReceived.Add(fun e -> e.Data |> output)
                p.ErrorDataReceived.Add(fun e -> e.Data |> output)

                p.StartInfo.UseShellExecute <- false
                p.StartInfo.CreateNoWindow <- true
                p.StartInfo.WindowStyle <- System.Diagnostics.ProcessWindowStyle.Hidden                
                //* Start process and handlers
                p.Start()
                p.BeginOutputReadLine()
                p.BeginErrorReadLine()
                p.WaitForExit()
                p.ExitCode = 0                
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
                        senderName = "FStarBuilTask")
                engine.LogErrorEvent(error)
                false            
        
        override this.HostObject
            with get () = host
            and set v = host <- v

        override this.BuildEngine
            with get () = engine
            and set v =  engine <- v
