using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;
using System.Windows.Input;
using Bindery.Implementations.Basic;
using Bindery.Interfaces;

namespace Bindery.Implementations
{
    internal class ControlObservableBinder<TSource, TControl, TArg>
        : BasicControlObservableBinder<TSource, TControl, TArg>,
        IControlObservableBinder<TSource, TControl, TArg>
        where TSource : INotifyPropertyChanged
        where TControl : IBindableComponent
    {

        public ControlObservableBinder(BasicControlBinder<TSource, TControl> parent, IObservable<TArg> observable)
            : base(parent, observable)
        {
        }


        IControlBinder<TSource, TControl> IControlObservableBinder<TSource, TControl, TArg>.Execute(ICommand command)
        {
            return (IControlBinder<TSource, TControl>)Execute(command);
        }

        IControlBinder<TSource, TControl> IControlObservableBinder<TSource, TControl, TArg>.Execute<TCommandArg>(ICommand command, Func<TArg, TCommandArg> conversion)
        {
            return (IControlBinder<TSource, TControl>)Execute(command, conversion);
        }

        IControlBinder<TSource, TControl> IControlObservableBinder<TSource, TControl, TArg>.Set(Expression<Func<TSource, TArg>> member)
        {
            return (IControlBinder<TSource, TControl>)Set(member);
        }

        IControlBinder<TSource, TControl> IControlObservableBinder<TSource, TControl, TArg>.Set<TSourceProp>(Expression<Func<TSource, TSourceProp>> member, Func<TArg, TSourceProp> conversion)
        {
            return (IControlBinder<TSource, TControl>)Set(member, conversion);
        }
    }
}
