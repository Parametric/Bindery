using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace Bindery.Interfaces
{
    public interface IBindableBinder<TSource, TBindable>
        where TSource : INotifyPropertyChanged
        where TBindable : IBindableComponent
    {
        IBindablePropertyBinder<TSource, TBindable, TProp> Property<TProp>(Expression<Func<TBindable, TProp>> member);

        IBindableEventBinder<TSource, TBindable> Event(Func<TBindable, Action<EventHandler>> getAddHandler);

        IBindableEventBinder<TSource, TBindable> Event<TEventArgs>(Func<TBindable, Action<EventHandler<TEventArgs>>> getAddHandler);

        IBindableEventBinder<TSource, TBindable> Event<TEventArgs, THandler>(Func<TBindable, Action<THandler>> getAddHandler);
    }
}
