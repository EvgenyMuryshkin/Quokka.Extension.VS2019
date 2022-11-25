using System;

namespace Quokka.Extension.VS2022
{
    internal class guidQuokkaExtensionVS2022PackageIds
    {
        public const string PackageGuidString = "ABE561FC-10C6-4D01-88B3-FACF816A3600";
        public static Guid QuokkaOutputWindowId = Guid.Parse("ECF6520A-9E03-4E6E-84D8-B0BA9DAF57B0");

        public static readonly Guid guidQuokkaExtensionVS2022PackageCmdSet = new Guid("9C1C773C-281E-4117-9C3C-7C1784D02C26");
        public const int cmdidCancelRunMethodCommand = 0x1200;
        public const int cmdidRerunExtensionMethodCommand = 0x1201;
        public const int cmdidExploreCommand = 0x1202;
        public const int cmdidReloadCommand = 0x1203;

        public const int ShowQuokkaExplorerCommandId = 0x100;
        public const int QuokkaExplorerMenuCommandId = 0x0101;

        public const int cmdidMyDynamicStart_Translate = 0x2000;
        public const int cmdidMyDynamicStart_BitStream = 0x3000;
        public const int cmdidMyDynamicStart_Program = 0x4000;
        public const int cmdidMyDynamicStart_Generic = 0x5000;

        public const int QuokkaExplorerToolbarId = 0x1100;
        public const int QuokkaExplorerToolbarGroupId = 0x1101;
    }
}
