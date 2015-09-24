using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FStarProject
{
    static class FStarProjectPackageGuids
    {
        public const string guidFStarProjectPkgString =
            "96bf4c26-d94e-43bf-a56a-f8500b52bfad";
        public const string guidFStarProjectCmdSetString =
            "72c23e1d-f389-410a-b5f1-c938303f1391";
        public const string guidFStarProjectFactoryString =
            "471EC4BB-E47E-4229-A789-D1F5F83B52D4";

        public static readonly Guid guidFStarProjectCmdSet =
            new Guid(guidFStarProjectCmdSetString);
        public static readonly Guid guidFStarProjectFactory =
            new Guid(guidFStarProjectFactoryString);
    }
}
