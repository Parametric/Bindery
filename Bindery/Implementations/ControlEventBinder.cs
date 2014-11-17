using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;
using System.Windows.Input;
using Bindery.Implementations.Basic;
using Bindery.Interfaces;

namespace Bindery.Implementations
{
    internal class ControlEventBinder<TSource, TControl, TEventArgs> : 
        BasicControlEventBinder<TSource, TControl, TEventArgs>, 
        IControlEventBinder<TSource, TControl, TEventArgs>
        where TSource : INotifyPropertyChanged
        where TControl : IBindableComponent 
    {
        public ControlEventBinder(ControlBinder<TSource, TControl> parent, string eventName) : base(parent, eventName)
        {
        }

        IControlBinder<TSource, TControl> IControlEventBinder<TSource, TControl, TEventArgs>.Execute(ICommand command)
        {
            return (IControlBinder<TSource, TControl>) Execute(command);
        }

        IControlBinder<TSource, TControl> IControlEventBinder<TSource, TControl, TEventArgs>.Execute<TConverted>(ICommand command, Func<TEventArgs, TConverted> conversion)
        {
            return (IControlBinder<TSource, TControl>)Execute(command, conversion);
        }

        IControlBinder<TSource, TControl> IControlEventBinder<TSource, TControl, TEventArgs>.Set<TSourceProp>(Expression<Func<TSource, TSourceProp>> propertyMember, Func<TEventArgs, TSourceProp> conversion)
        {
            return (IControlBinder<TSource, TControl>) Set(propertyMember, conversion);
        }
    }
}