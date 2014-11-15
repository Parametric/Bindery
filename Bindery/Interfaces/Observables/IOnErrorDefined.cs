using System;
using System.ComponentModel;
using System.Threading;

namespace Bindery.Interfaces.Observables
{
    public interface IOnErrorDefined<TSource> : ISubscriptionComplete<TSource> 
        where TSource : INotifyPropertyChanged
    {
        IOnCompleteDefined<TSource> OnComplete(Action onComplete);
        ISubscriptionComplete<TSource> CancellationToken(CancellationToken token);
    }
}