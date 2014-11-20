using System;
using System.Linq.Expressions;
using System.Windows.Input;
using Bindery.Interfaces.Observables;

namespace Bindery.Interfaces
{
    public interface IObservableBinder<TSource, TArg> 
    {
        IOnNextDefined<TSource> OnNext(Action<TArg> onNext);
        IObservableBinder<TSource, TOut> Transform<TOut>(Func<IObservable<TArg>, IObservable<TOut>> transform);
        ISourceBinder<TSource> Subscribe(Action<TArg> action);
        ISourceBinder<TSource> Execute(ICommand command);
        ISourceBinder<TSource> Execute(ICommand command, object commandParameter);
        ISourceBinder<TSource> Execute(ICommand command, Func<TArg, object> toCommandParameter);
        ISourceBinder<TSource> Execute(ICommand command, Func<object> getCommandParameter);
        ISourceBinder<TSource> Set(Expression<Func<TSource, TArg>> member);
        ISourceBinder<TSource> Set<TProp>(Expression<Func<TSource, TProp>> member, Func<TArg, TProp> conversion);
    }
}