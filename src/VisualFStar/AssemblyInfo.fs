namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("VisualFStar")>]
[<assembly: AssemblyProductAttribute("VisualFStar")>]
[<assembly: AssemblyDescriptionAttribute("Support for F* in Visual Studio IDE")>]
[<assembly: AssemblyVersionAttribute("1.0")>]
[<assembly: AssemblyFileVersionAttribute("1.0")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "1.0"
