using System;
using System.ComponentModel;
using System.Threading;

namespace Bindery.Interfaces.Observables
{
    public interface IOnNextDefined<TSource> : ISubscriptionComplete<TSource>
    {
        IOnErrorDefined<TSource> OnError(Action<Exception> onError);
        IOnCompleteDefined<TSource> OnComplete(Action onComplete);
        ISubscriptionComplete<TSource> CancellationToken(CancellationToken token);
    }
}