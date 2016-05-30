using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSExpInstanceReset
{
    internal sealed partial class GuidList
    {
        public const string PackageGuidString = "0440cf56-007c-45c1-aeba-ae8ca81fd5fa";
        public const string CommandSet = "2ef51c7c-6cb4-45ff-823d-6ec9d8bea21d";
        public static Guid guidResetVSExpInstanceCommandPackage = new Guid(PackageGuidString);
        public static Guid guidResetVSExpInstanceCommandPackageCmdSet = new Guid(CommandSet);

    }

    internal sealed partial class PackageIds
    {
        public const int ResetExpCacheId = 0x0100;
    }
}
