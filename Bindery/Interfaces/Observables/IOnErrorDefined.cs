using System;
using System.ComponentModel;
using System.Threading;

namespace Bindery.Interfaces.Observables
{
    public interface IOnErrorDefined<TSource> : ISubscriptionComplete<TSource> 
    {
        IOnCompleteDefined<TSource> OnComplete(Action onComplete);
        ISubscriptionComplete<TSource> CancellationToken(CancellationToken token);
    }
}