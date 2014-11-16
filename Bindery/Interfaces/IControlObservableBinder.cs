using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;
using System.Windows.Input;

namespace Bindery.Interfaces
{
    public interface IControlObservableBinder<TSource, TControl, TArg>
        where TSource : INotifyPropertyChanged
        where TControl : IBindableComponent
    {
        IControlBinder<TSource, TControl> Execute(Func<TSource, ICommand> commandMember);
        IControlBinder<TSource, TControl> Execute<TCommandArg>(Func<TSource, ICommand> commandMember, Func<TArg, TCommandArg> conversion);
        IControlBinder<TSource, TControl> Set(Expression<Func<TSource, TArg>> member);
        IControlBinder<TSource, TControl> Set<TSourceProp>(Expression<Func<TSource, TSourceProp>> member, Func<TArg,TSourceProp> conversion);
    }
}