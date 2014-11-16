using System;

namespace Bindery.Interfaces.Observables
{
    public interface IEventObservableFactory
    {
        IObservable<EventArgs> Event(string eventName);
        IObservable<TEventArgs> Event<TEventArgs>(string eventName);
    }
}