using System;
using System.Threading;

namespace Bindery.Interfaces.Subscriptions
{
    /// <summary>
    ///     Subscription context after OnError has been defined
    /// </summary>
    public interface IOnErrorDefined : ISubscriptionComplete
    {
        /// <summary>
        ///     Define the action to take on IObserver.OnComplete
        /// </summary>
        /// <param name="onComplete">The action</param>
        /// <returns>The next step in the subscription syntax</returns>
        IOnCompleteDefined OnComplete(Action onComplete);

        /// <summary>
        ///     Define a cancellation token for the subscription
        /// </summary>
        /// <param name="token">The cancellation token</param>
        /// <returns>Subscription complete indicator</returns>
        ISubscriptionComplete CancellationToken(CancellationToken token);
    }
}