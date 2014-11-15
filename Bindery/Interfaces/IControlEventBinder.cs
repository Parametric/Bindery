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
        IControlBinder<TSource, TControl> Executes(Func<TSource, ICommand> commandMember);
        IControlBinder<TSource, TControl> Executes<TConverted>(Func<TSource, ICommand> commandMember, Func<TEventArgs, TConverted> conversion);
        IControlBinder<TSource, TControl> UpdateSource<TSourceProp>(Expression<Func<TSource, TSourceProp>> propertyMember, Func<TEventArgs, TSourceProp> conversion);
    }
}