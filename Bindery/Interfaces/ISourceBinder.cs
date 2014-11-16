using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace Bindery.Interfaces
{
    public interface ISourceBinder<TSource> : IDisposable 
        where TSource:INotifyPropertyChanged
    {
        IControlBinder<TSource, TControl> Control<TControl>(TControl control) where TControl : IBindableComponent;
        ITargetBinder<TSource, TTarget> Target<TTarget>(TTarget target) where TTarget : class;
        ISourcePropertyBinder<TSource, TProp> Property<TProp>(Expression<Func<TSource, TProp>> member);
        ISourceObservableBinder<TSource, TArg> Observe<TArg>(Func<TSource, IObservable<TArg>> observableMember);
    }
}