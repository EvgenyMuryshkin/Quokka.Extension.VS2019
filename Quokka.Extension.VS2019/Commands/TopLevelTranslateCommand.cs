﻿using Quokka.Extension.Interface;
using Quokka.Extension.Interop;
using Quokka.Extension.Scaffolding;

namespace Quokka.Extension.VS2019
{
    internal class TopLevelTranslateCommand : DynamicItemMenuCommandFactory
    {
        public TopLevelTranslateCommand(
            ExtensionDeps deps,
            IExtensionInvocationService invocationService,
            IExtensionsCacheService extensionsCacheService
            ) : base(deps, invocationService, extensionsCacheService, guidQuokkaExtensionVS2019PackageIds.guidQuokkaExtensionVS2019PackageCmdSet, guidQuokkaExtensionVS2019PackageIds.cmdidMyDynamicStart_Translate, TopLevelIcon.Translate)
        {
        }
    }
}
