using Quokka.Extension.Services;

namespace Quokka.Extension.VS2019
{
    struct DynamicItemInfo
    {
        public DynamicItemInfo(int id, ExtensionMethodInfo icon)
        {
            Id = id;
            Icon = icon;
        }

        public int Id;
        public ExtensionMethodInfo Icon;
    }
}
