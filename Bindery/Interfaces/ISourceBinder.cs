using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace Bindery.Interfaces
{
    public interface ISourceBinder<T> : IDisposable 
        where T:INotifyPropertyChanged
    {
        ISourcePropertyBinder<T, TProp> Property<TProp>(Expression<Func<T, TProp>> member);
        IControlBinder<T, TControl> Control<TControl>(TControl control) where TControl : Control;
        IBindableBinder<T, TBindable> Bindable<TBindable>(TBindable bindable) where TBindable : IBindableComponent;
    }
}