using System;

namespace Bindery.Interfaces.Observables
{
    public interface IEventObservableFactory
    {
        IObservable<EventArgs> Create(string eventName);
        IObservable<TEventArgs> Create<TEventArgs>(string eventName);
    }
}