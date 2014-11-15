using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace Bindery.Interfaces
{
    public interface ISourceBinder<TSource> : IDisposable 
        where TSource:INotifyPropertyChanged
    {
        ISourcePropertyBinder<TSource, TProp> Property<TProp>(Expression<Func<TSource, TProp>> member);
        IControlBinder<TSource, TControl> Control<TControl>(TControl control) where TControl : IBindableComponent;
        ISourceObservableBinder<TSource, TArg> Observe<TArg>(Func<TSource, IObservable<TArg>> observableMember);
    }
}