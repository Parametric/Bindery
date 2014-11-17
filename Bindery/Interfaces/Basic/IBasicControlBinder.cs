using System;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace Bindery.Interfaces.Basic
{
    public interface IBasicControlBinder<TSource, TControl> 
        where TControl : IBindableComponent
    {
        IBasicControlPropertyBinder<TSource, TControl, TProp> Property<TProp>(Expression<Func<TControl, TProp>> member);
        IBasicControlEventBinder<TSource, TControl, EventArgs> OnEvent(string eventName);
        IBasicControlEventBinder<TSource, TControl, TEventArgs> OnEvent<TEventArgs>(string eventName);
        IBasicControlObservableBinder<TSource, TControl, TArg> On<TArg>(Func<TControl, IObservable<TArg>> observableMember);
    }
}