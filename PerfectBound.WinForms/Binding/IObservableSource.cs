using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace PerfectBound.WinForms.Binding
{
    public interface IObservableSource<T> : IDisposable 
        where T:INotifyPropertyChanged
    {
        IObservableSourceProperty<T, TProp> Property<TProp>(Expression<Func<T, TProp>> member);
        IObservableSourceControl<T, TControl> Control<TControl>(TControl control) where TControl : Control;
    }
}