using System.ComponentModel;
using System.Threading;

namespace Bindery.Interfaces.Observables
{
    public interface IOnCompleteDefined<TSource> : ISubscriptionComplete<TSource> 
        where TSource : INotifyPropertyChanged
    {
        ISubscriptionComplete<TSource> CancellationToken(CancellationToken token);
    }
}