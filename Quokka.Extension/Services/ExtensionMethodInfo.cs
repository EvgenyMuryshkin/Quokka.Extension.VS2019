using Quokka.Extension.Interop;

namespace Quokka.Extension.Services
{
    public class ExtensionMethodInfo
    {
        public string Project;
        public string Class;
        public string Method;
        public ExtensionMethodIcon Icon;
        public string Title;

        public string DisplayTitle => Title ?? $"{Class}.{Method}";
        public string DisplayMethod => Title ?? Method;

        public override string ToString()
        {
            return $"{Project}: {Class}.{Method}";
        }
    }
}
