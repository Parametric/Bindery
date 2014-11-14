using System;
using System.ComponentModel;

namespace PerfectBound.WinForms.Interfaces
{
    public interface ISourcePropertyBinder<TSource, out TProp> where TSource : INotifyPropertyChanged
    {
        ISourceBinder<TSource> OnChanged(Action<TProp> action);
    }
}