using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FStarProject
{
    static class FSharpProjectPackageGuids
    {
        public const string guidFSharpProjectPkgString =
            "96bf4c26-d94e-43bf-a56a-f8500b52bf5d";
        public const string guidFSharpProjectCmdSetString =
            "72c23e1d-f389-410a-b5f1-c938303f1351";
        public const string guidFSharpProjectFactoryString =
            "471EC4BB-E47E-4229-A789-D1F5F83B5254";

        public static readonly Guid guidFSharpProjectCmdSet =
            new Guid(guidFSharpProjectCmdSetString);
        public static readonly Guid guidFSharpProjectFactory =
            new Guid(guidFSharpProjectFactoryString);
    }
}
