using System;

namespace Bindery.Interfaces
{
    /// <summary>
    ///     Factory for creating observable from an event
    /// </summary>
    public interface IEventObservableFactory
    {
        /// <summary>
        ///     Create an IObservable{EventArgs} for an event
        /// </summary>
        /// <param name="eventName">The name of the event</param>
        /// <returns>An observable</returns>
        IObservable<EventContext<EventArgs>> Event(string eventName);

        /// <summary>
        ///     Create an IObservable{TEventArgs} for an event
        /// </summary>
        /// <typeparam name="TEventArgs">The type of the event's argument parameter</typeparam>
        /// <param name="eventName">The name of the event</param>
        /// <returns>An observable</returns>
        IObservable<EventContext<TEventArgs>> Event<TEventArgs>(string eventName);
    }
}