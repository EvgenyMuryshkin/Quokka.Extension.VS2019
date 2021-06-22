using Quokka.Extension.Interface;
using Quokka.Extension.Interop;
using Quokka.Extension.Scaffolding;

namespace Quokka.Extension.VS2019
{
    internal class TopLevelGenericCommand : DynamicItemMenuCommandFactory
    {
        public TopLevelGenericCommand(
            ExtensionDeps deps,
            IExtensionInvocationService invocationService,
            IExtensionsCacheService extensionsCacheService
            ) : base(deps, invocationService, extensionsCacheService, guidQuokkaExtensionVS2019PackageIds.guidQuokkaExtensionVS2019PackageCmdSet, guidQuokkaExtensionVS2019PackageIds.cmdidMyDynamicStart_Generic, TopLevelIcon.Generic)
        {
        }
    }
}
