using System.Threading;

namespace Bindery.Interfaces.Subscriptions
{
    /// <summary>
    ///     Subscription context after OnComplete has been defined
    /// </summary>
    public interface IOnCompleteDefined : ISubscriptionComplete
    {
        /// <summary>
        ///     Define a cancellation token for the subscription
        /// </summary>
        /// <param name="token">The cancellation token</param>
        /// <returns>Subscription complete indicator</returns>
        ISubscriptionComplete CancellationToken(CancellationToken token);
    }
}