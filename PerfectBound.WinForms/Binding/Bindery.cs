using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace PerfectBound.WinForms.Binding
{
    public static class Bindery
    {
        public static IObservableSource<T> ObservableSource<T>(T source) where T : INotifyPropertyChanged
        {
            return new ObservableSource<T>(source);
        }
    }
}
