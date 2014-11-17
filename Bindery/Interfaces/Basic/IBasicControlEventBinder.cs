using System;
using System.Linq.Expressions;
using System.Windows.Forms;
using System.Windows.Input;

namespace Bindery.Interfaces.Basic
{
    public interface IBasicControlEventBinder<TSource, TControl, out TEventArgs>
        where TControl : IBindableComponent
    {
        IBasicControlBinder<TSource, TControl> Execute(Func<TSource, ICommand> commandMember);
        IBasicControlBinder<TSource, TControl> Execute<TCommandArg>(Func<TSource, ICommand> commandMember, Func<TEventArgs, TCommandArg> conversion);
        IBasicControlBinder<TSource, TControl> Set<TSourceProp>(Expression<Func<TSource, TSourceProp>> propertyMember, Func<TEventArgs, TSourceProp> conversion);
        IObservable<TEventArgs> AsObservable();
    }
}