using System;

namespace Bindery.Interfaces
{
    public interface IEventObservableFactory
    {
        IObservable<EventArgs> Event(string eventName);
        IObservable<TEventArgs> Event<TEventArgs>(string eventName);
    }
}