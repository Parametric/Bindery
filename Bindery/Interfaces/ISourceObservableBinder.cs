using System;
using System.ComponentModel;

namespace Bindery.Interfaces
{
    public interface ISourceObservableBinder<TSource, out TArg> 
        where TSource : INotifyPropertyChanged
    {
        ISourceBinder<TSource> OnNext(Action<TArg> action);
    }
}