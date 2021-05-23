namespace Quokka.Extension
{
    public interface IExtensionLogger
    {
        void Write(string message);
        void WriteLine(string message);
    }
}
