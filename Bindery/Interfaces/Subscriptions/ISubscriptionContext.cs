using System;

namespace Bindery.Interfaces.Subscriptions
{
    /// <summary>
    ///     An observable subscription context
    /// </summary>
    /// <typeparam name="T">The observable type</typeparam>
    public interface ISubscriptionContext<out T>
    {
        /// <summary>
        ///     Define the action to call on IObserver.OnNext
        /// </summary>
        /// <param name="action">The action</param>
        /// <returns>The next step in the subscription syntax</returns>
        IOnNextDefined OnNext(Action<T> action);
    }
}