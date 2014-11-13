using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace PerfectBound.WinForms.Interfaces
{
    public interface IObservableSourceBindable<TSource, TBindable>
        where TSource : INotifyPropertyChanged
        where TBindable : IBindableComponent
    {
        IObservableSourceBindableProperty<TSource, TBindable, TProp> Property<TProp>(Expression<Func<TBindable, TProp>> member);
    }
}
