using System;
using System.Linq.Expressions;

namespace Bindery.Interfaces
{
    public interface ITargetBinder<TSource, TTarget>
    {
        ITargetPropertyBinder<TSource,TTarget,TProp> Property<TProp>(Expression<Func<TTarget, TProp>> member);
        IObservableBinder<TSource, EventArgs> OnEvent(string eventName);
        IObservableBinder<TSource, TEventArgs> OnEvent<TEventArgs>(string eventName);
        IObservableBinder<TSource, TArg> Observe<TArg>(Func<TTarget, IObservable<TArg>> observableMember);
    }
}