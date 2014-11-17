using System;
using System.Linq.Expressions;
using System.Windows.Forms;
using System.Windows.Input;

namespace Bindery.Interfaces.Basic
{
    public interface IBasicControlObservableBinder<TSource, TControl, TArg> where TControl 
        : IBindableComponent
    {
        IBasicControlBinder<TSource, TControl> Execute(Func<TSource, ICommand> commandMember);
        IBasicControlBinder<TSource, TControl> Execute<TCommandArg>(Func<TSource, ICommand> commandMember, Func<TArg, TCommandArg> conversion);
        IBasicControlBinder<TSource, TControl> Set(Expression<Func<TSource, TArg>> member);
        IBasicControlBinder<TSource, TControl> Set<TSourceProp>(Expression<Func<TSource, TSourceProp>> member, Func<TArg, TSourceProp> conversion);
    }
}