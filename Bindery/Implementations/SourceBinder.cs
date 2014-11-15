using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Windows.Forms;
using Bindery.Interfaces;

namespace Bindery.Implementations
{
    internal class SourceBinder<T> : ISourceBinder<T> where T : INotifyPropertyChanged
    {
        public T Object { get; private set; }
        public IObservable<PropertyChangedEventArgs> PropertyChangedObservable { get; private set; }
        private readonly List<IDisposable> _subscriptions;

        public SourceBinder(T obj)
        {
            Object = obj;
            PropertyChangedObservable = Observable.FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                argsAction => (sender, e) => argsAction(e),
                handler => Object.PropertyChanged += handler,
                handler => Object.PropertyChanged -= handler);
            _subscriptions = new List<IDisposable>();
        }


        public ISourcePropertyBinder<T, TProp> Property<TProp>(Expression<Func<T, TProp>> member)
        {
            return new SourcePropertyBinder<T,TProp>(this, member);
        }

        public IControlBinder<T, TControl> Control<TControl>(TControl control) where TControl : IBindableComponent
        {
            return new ControlBinder<T,TControl>(this, control);
        }

        public void Dispose()
        {
            _subscriptions.ForEach(x=>x.Dispose());
        }

        public void AddSubscription(IDisposable subscription)
        {
            _subscriptions.Add(subscription);
        }
    }
}