using Quokka.Extension.Interface;
using Quokka.Extension.Interop;
using Quokka.Extension.Scaffolding;

namespace Quokka.Extension.VS2019
{
    internal class TopLevelProgramCommand : DynamicExtensionMethodsMenuService
    {
        public TopLevelProgramCommand(
            ExtensionDeps deps,
            IExtensionInvocationService invocationService,
            IExtensionsCacheService extensionsCacheService
            ) : base(deps, invocationService, extensionsCacheService, guidQuokkaExtensionVS2019PackageIds.cmdidMyDynamicStart_Program, TopLevelIcon.Program)
        {
        }
    }
}
