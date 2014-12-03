using System;
using System.Linq.Expressions;

namespace Bindery.Interfaces.Binders
{
    /// <summary>
    /// Binder for a binding target object
    /// </summary>
    /// <typeparam name="TSource">The type of the binding source</typeparam>
    /// <typeparam name="TTarget">The type of the binding target</typeparam>
    public interface ITargetBinder<TSource, TTarget>
    {
        /// <summary>
        /// Bind to a target property
        /// </summary>
        /// <typeparam name="TProp">The property type</typeparam>
        /// <param name="member">Expression that specifies the target's property</param>
        /// <returns>A target property binder</returns>
        ITargetPropertyBinder<TSource, TTarget, TProp> Property<TProp>(Expression<Func<TTarget, TProp>> member);

        /// <summary>
        /// Create an IObservable{EventArgs} for one of the target's events
        /// </summary>
        /// <param name="eventName">The name of the event</param>
        /// <returns>An observable binder</returns>
        IObservableBinder<TSource, EventArgs> OnEvent(string eventName);

        /// <summary>
        /// Create an IObservable{TEventArgs} for one of the target's events
        /// </summary>
        /// <typeparam name="TEventArgs">The type of the event's argument parameter</typeparam>
        /// <param name="eventName">The name of the event</param>
        /// <returns>An observable binder</returns>
        IObservableBinder<TSource, TEventArgs> OnEvent<TEventArgs>(string eventName);
    }
}