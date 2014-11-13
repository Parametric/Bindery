using System;
using System.ComponentModel;

namespace PerfectBound.WinForms.Binding
{
    public interface IObservableSourceProperty<TSource, out TProp> where TSource : INotifyPropertyChanged
    {
        IObservableSource<TSource> OnChanged(Action<TProp> action);
    }
}