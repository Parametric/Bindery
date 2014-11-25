using System;
using System.Threading;

namespace Bindery.Interfaces.Subscriptions
{
    public interface IOnErrorDefined : ISubscriptionComplete
    {
        IOnCompleteDefined OnComplete(Action onComplete);
        ISubscriptionComplete CancellationToken(CancellationToken token);
    }
}