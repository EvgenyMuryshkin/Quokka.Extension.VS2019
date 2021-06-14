using Quokka.Extension.Interface;
using System.Collections.Generic;
using System.Linq;

namespace Quokka.Extension.Services
{
    public class InvocationCacheService : IInvocationCacheService
    {
        List<ExtensionMethodInfo> invokeHistory = new List<ExtensionMethodInfo>();

        public void Clear()
        {
            invokeHistory.Clear();
        }

        public ExtensionMethodInfo Peek()
        {
            return invokeHistory.Any() ? invokeHistory[0] : null;
        }

        public void Push(ExtensionMethodInfo info)
        {
            invokeHistory.RemoveAll(p => p.ToString() == info.ToString());
            invokeHistory.Insert(0, info);
        }
    }
}
