using System;

namespace Quokka.Extension.VS2019
{
    internal class guidQuokkaExtensionVS2019PackageIds
    {
        public const string PackageGuidString = "d9ff7f11-8ffd-4154-9fb2-2d1857864b98";
        public static Guid QuokkaOutputWindowId = Guid.Parse("51db306c-dd34-4cdf-afa7-3b35946cb4e1");

        public static readonly Guid guidQuokkaExtensionVS2019PackageCmdSet = new Guid("4bb7016f-f3ce-4305-91c2-2493253e2325");
        public const int cmdidCancelRunMethodCommand = 0x1200;
        public const int cmdidRerunExtensionMethodCommand = 0x1201;
        public const int cmdidExploreCommand = 0x1202;
        public const int cmdidInvokeExtensionMethodCommandId = 0x100;

        public const uint cmdidMyDynamicStartCommand1 = 0x2001;
        public const uint cmdidMyDynamicStartCommand2 = 0x3001;
        public const uint cmdidMyDynamicStartCommand3 = 0x4001;
    }
}
