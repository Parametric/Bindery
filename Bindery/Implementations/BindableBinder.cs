using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;
using Bindery.Interfaces;

namespace Bindery.Implementations
{
    internal class BindableBinder<TSource, TBindable> : IBindableBinder<TSource, TBindable> 
        where TSource : INotifyPropertyChanged
        where TBindable : IBindableComponent
    {
        public SourceBinder<TSource> SourceBinder { get; private set; }
        public TBindable Bindable { get; private set; }

        public BindableBinder(SourceBinder<TSource> sourceBinder, TBindable bindable)
        {
            SourceBinder = sourceBinder;
            Bindable = bindable;
        }

        public IBindablePropertyBinder<TSource, TBindable, TProp> Property<TProp>(Expression<Func<TBindable, TProp>> member)
        {
            return new BindablePropertyBinder<TSource, TBindable, TProp>(this, member);
        }

        public IBindableEventBinder<TSource, TBindable> Event(Func<TBindable, Action<EventHandler>> getAddHandler)
        {
            return new BindableEventBinder<TSource, TBindable, EventArgs, EventHandler>(this, getAddHandler);
        }

        public IBindableEventBinder<TSource, TBindable> Event<TEventArgs>(Func<TBindable, Action<EventHandler<TEventArgs>>> getAddHandler)
        {
            return new BindableEventBinder<TSource, TBindable, TEventArgs, EventHandler<TEventArgs>>(this, getAddHandler);
        }

        public IBindableEventBinder<TSource, TBindable> Event<TEventArgs, THandler>(Func<TBindable, Action<THandler>> getAddHandler)
        {
            return new BindableEventBinder<TSource, TBindable, TEventArgs, THandler>(this, getAddHandler);
        }

        public void AddDataBinding(Binding binding)
        {
            Bindable.DataBindings.Add(binding);
        }

        internal Binding CreateBinding(string controlPropertyName, string sourcePropertyName, ControlUpdateMode controlUpdateMode, DataSourceUpdateMode dataSourceUpdateMode)
        {
            return new Binding(controlPropertyName, SourceBinder.Object, sourcePropertyName)
            {
                ControlUpdateMode = controlUpdateMode,
                DataSourceUpdateMode = dataSourceUpdateMode
            };
        }
    }
}