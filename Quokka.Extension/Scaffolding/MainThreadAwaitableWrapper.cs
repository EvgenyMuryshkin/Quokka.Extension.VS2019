using System;
using System.Runtime.CompilerServices;

namespace Quokka.Extension.Scaffolding
{
    public struct MainThreadAwaitableWrapper
    {
        private readonly MainThreadAwaiterWrapper _wrapper;
        public MainThreadAwaitableWrapper(ICriticalNotifyCompletion criticalNotifyCompletion)
        {
            _wrapper = new MainThreadAwaiterWrapper(criticalNotifyCompletion);
        }

        public MainThreadAwaiterWrapper GetAwaiter() => _wrapper;
    }

    public struct MainThreadAwaiterWrapper : ICriticalNotifyCompletion, INotifyCompletion
    {
        private readonly ICriticalNotifyCompletion _criticalNotifyCompletion;
        public MainThreadAwaiterWrapper(ICriticalNotifyCompletion criticalNotifyCompletion)
        {
            _criticalNotifyCompletion = criticalNotifyCompletion;
            IsCompleted = false;
        }

        public bool IsCompleted { get; set; }

        public void GetResult()
        {
            // what should be there?
        }

        public void OnCompleted(Action continuation)
        {
            _criticalNotifyCompletion.OnCompleted(continuation);
            IsCompleted = true;
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            _criticalNotifyCompletion.UnsafeOnCompleted(continuation);
        }
    }
}
