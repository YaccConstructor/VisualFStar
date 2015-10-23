namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("VisualFStarCore")>]
[<assembly: AssemblyProductAttribute("VisualFStar")>]
[<assembly: AssemblyDescriptionAttribute("Support for F* in Visual Studio IDE")>]
[<assembly: AssemblyVersionAttribute("0.0.1")>]
[<assembly: AssemblyFileVersionAttribute("0.0.1")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "0.0.1"
