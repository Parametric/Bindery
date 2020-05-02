using System;
using System.Linq.Expressions;

namespace Bindery.Interfaces.Binders
{
    /// <summary>
    ///     Binder for a binding target object
    /// </summary>
    /// <typeparam name="TSource">The type of the binding source</typeparam>
    /// <typeparam name="TTarget">The type of the binding target</typeparam>
    public interface ITargetBinder<TSource, TTarget> 
        // Inherit from ISourceBinder to make chaining calls easier
        : ISourceBinder<TSource>
    {
        /// <summary>
        ///     Bind to a target property
        /// </summary>
        /// <typeparam name="TProp">The property type</typeparam>
        /// <param name="member">Expression that specifies the target's property</param>
        /// <returns>A target property binder</returns>
        ITargetPropertyBinder<TSource, TTarget, TProp> Property<TProp>(Expression<Func<TTarget, TProp>> member);

        /// <summary>
        ///     Create an IObservable{(object, EventArgs)} for one of the target's events
        /// </summary>
        /// <param name="eventName">The name of the event</param>
        /// <returns>An observable binder</returns>
        IObservableBinder<TSource, (object Sender, EventArgs Args)> Event(string eventName);

        /// <summary>
        ///     Create an IObservable{(object, TEventARgs)} for one of the target's events
        /// </summary>
        /// <typeparam name="TEventArgs">The type of the event's argument parameter</typeparam>
        /// <param name="eventName">The name of the event</param>
        /// <returns>An observable binder</returns>
        IObservableBinder<TSource, (object Sender, TEventArgs Args)> Event<TEventArgs>(string eventName);

        [Obsolete("Use Event()", true)]
        IObservableBinder<TSource, (object Sender, EventArgs Args)> OnEvent(string eventName);
        [Obsolete("Use Event() and specify event name", true)]
        IObservableBinder<TSource, (object Sender, TEventArgs Args)> OnEvent<TEventArgs>();
        [Obsolete("Use Event()", true)]
        IObservableBinder<TSource, (object Sender, TEventArgs Args)> OnEvent<TEventArgs>(string eventName);
    }
}