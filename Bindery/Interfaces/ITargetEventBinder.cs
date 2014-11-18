using System;
using System.Linq.Expressions;
using System.Windows.Input;

namespace Bindery.Interfaces
{
    public interface ITargetEventBinder<TSource, TTarget, out TEventArgs>
    {
        ITargetBinder<TSource, TTarget> Execute(ICommand command);
        ITargetBinder<TSource, TTarget> Execute<TCommandArg>(ICommand command, Func<TEventArgs, TCommandArg> conversion);
        ITargetBinder<TSource, TTarget> Set<TSourceProp>(Expression<Func<TSource, TSourceProp>> propertyMember, Func<TEventArgs, TSourceProp> conversion);
        IObservable<TEventArgs> AsObservable();
    }
}