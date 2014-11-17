using System;

namespace Bindery.Interfaces
{
    public interface ITrigger<T> : IDisposable
    {
        IObservable<T> Observable { get; }
        void Push(T arg);
    }
}