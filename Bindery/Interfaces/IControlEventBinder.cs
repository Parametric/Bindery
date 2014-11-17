using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;
using System.Windows.Input;

namespace Bindery.Interfaces
{
    public interface IControlEventBinder<TSource, TControl, out TEventArgs>
        where TControl : IBindableComponent 
        where TSource : INotifyPropertyChanged
    {
        IControlBinder<TSource, TControl> Execute(ICommand command);
        IControlBinder<TSource, TControl> Execute<TCommandArg>(ICommand command, Func<TEventArgs, TCommandArg> conversion);
        IControlBinder<TSource, TControl> Set<TSourceProp>(Expression<Func<TSource, TSourceProp>> propertyMember, Func<TEventArgs, TSourceProp> conversion);
        IObservable<TEventArgs> AsObservable();
    }
}