using Quokka.Extension.Interface;
using Quokka.Extension.Interop;
using Quokka.Extension.Scaffolding;

namespace Quokka.Extension.VS2022
{
    internal class TopLevelTranslateCommand : DynamicItemMenuCommandFactory
    {
        public TopLevelTranslateCommand(
            ExtensionDeps deps,
            IExtensionInvocationService invocationService,
            IExtensionsCacheService extensionsCacheService
            ) : base(deps, invocationService, extensionsCacheService, guidQuokkaExtensionVS2022PackageIds.guidQuokkaExtensionVS2022PackageCmdSet, guidQuokkaExtensionVS2022PackageIds.cmdidMyDynamicStart_Translate, TopLevelIcon.Translate)
        {
        }
    }
}
