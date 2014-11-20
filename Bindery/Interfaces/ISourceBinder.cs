using System;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace Bindery.Interfaces
{
    public interface ISourceBinder<TSource> : IDisposable
    {
        IControlBinder<TSource, TControl> Control<TControl>(TControl control) where TControl : IBindableComponent;
        ITargetBinder<TSource, TTarget> Target<TTarget>(TTarget target) where TTarget : class;
        IObservableBinder<TSource, TProp> Property<TProp>(Expression<Func<TSource, TProp>> member);
        IObservableBinder<TSource, TArg> Observe<TArg>(IObservable<TArg> observable);
        IObservableBinder<TSource, TArg> Observe<TArg>(Func<TSource, IObservable<TArg>> observableMember);
    }
}