using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;
using System.Windows.Input;

namespace Bindery.Interfaces
{
    public interface IControlEventBinder<TSource, TControl, out TEventArgs>
        where TSource : INotifyPropertyChanged
        where TControl : IBindableComponent
    {
        IControlBinder<TSource, TControl> Execute(Func<TSource, ICommand> commandMember);
        IControlBinder<TSource, TControl> Execute<TCommandArg>(Func<TSource, ICommand> commandMember, Func<TEventArgs, TCommandArg> conversion);
        IControlBinder<TSource, TControl> Set<TSourceProp>(Expression<Func<TSource, TSourceProp>> propertyMember, Func<TEventArgs, TSourceProp> conversion);
        IObservable<TEventArgs> AsObservable();
    }
}