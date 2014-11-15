using System.ComponentModel;

namespace Bindery.Interfaces.Observables
{
    public interface ISubscriptionComplete<TSource> 
        where TSource : INotifyPropertyChanged
    {
        ISourceBinder<TSource> Subscribe();
    }
}