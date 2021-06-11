using Quokka.Extension.Interop;

namespace Quokka.Extension
{
    public class ExtensionMethodInfo
    {
        public string Project;
        public string Class;
        public string Method;
        public ExtensionMethodIcon Icon;

        public override string ToString()
        {
            return $"{Project}: {Class}.{Method}";
        }
    }
}
