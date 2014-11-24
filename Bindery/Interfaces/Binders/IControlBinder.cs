using System;
using System.Linq.Expressions;
using System.Windows.Forms;
using System.Windows.Input;

namespace Bindery.Interfaces.Binders
{
    public interface IControlBinder<TSource, TControl>
        where TControl : IBindableComponent
    {
        IControlPropertyBinder<TSource, TControl, TProp> Property<TProp>(Expression<Func<TControl, TProp>> member);
        IObservableBinder<TSource, EventArgs> OnEvent(string eventName);
        IObservableBinder<TSource, TEventArgs> OnEvent<TEventArgs>(string eventName);
        IControlBinder<TSource, TControl> OnClick(ICommand command);
        IControlBinder<TSource, TControl> OnClick(ICommand command, Func<object> getParameter);
        IObservableBinder<TSource,TArg> Observe<TArg>(Func<TControl, IObservable<TArg>> observableMember);
    }

}
