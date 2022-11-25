using Quokka.Extension.Interface;
using Quokka.Extension.Interop;
using Quokka.Extension.Scaffolding;

namespace Quokka.Extension.VS2022
{
    internal class TopLevelBitStreamCommand : DynamicItemMenuCommandFactory
    {
        public TopLevelBitStreamCommand(
            ExtensionDeps deps,
            IExtensionInvocationService invocationService,
            IExtensionsCacheService extensionsCacheService
            ) : base(deps, invocationService, extensionsCacheService, guidQuokkaExtensionVS2022PackageIds.guidQuokkaExtensionVS2022PackageCmdSet, guidQuokkaExtensionVS2022PackageIds.cmdidMyDynamicStart_BitStream, TopLevelIcon.BitStream)
        {
        }
    }
}
