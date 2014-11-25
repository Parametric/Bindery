using System;
using System.Threading;

namespace Bindery.Interfaces.Subscriptions
{
    public interface IOnNextDefined : ISubscriptionComplete
    {
        IOnErrorDefined OnError(Action<Exception> onError);
        IOnCompleteDefined OnComplete(Action onComplete);
        ISubscriptionComplete CancellationToken(CancellationToken token);
    }
}