using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Windows.Forms;
using Bindery.Interfaces;

namespace Bindery.Implementations
{
    internal class SourceBinder<TSource> : ISourceBinder<TSource> 
        where TSource : INotifyPropertyChanged
    {
        public TSource Source { get; private set; }
        public IObservable<PropertyChangedEventArgs> PropertyChangedObservable { get; private set; }
        private readonly List<IDisposable> _subscriptions;

        public SourceBinder(TSource obj)
        {
            Source = obj;
            PropertyChangedObservable = Observable.FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                argsAction => (sender, e) => argsAction(e),
                handler => Source.PropertyChanged += handler,
                handler => Source.PropertyChanged -= handler);
            _subscriptions = new List<IDisposable>();
        }


        public ISourcePropertyBinder<TSource, TProp> Property<TProp>(Expression<Func<TSource, TProp>> member)
        {
            return new SourcePropertyBinder<TSource,TProp>(this, member);
        }

        public IControlBinder<TSource, TControl> Control<TControl>(TControl control) where TControl : IBindableComponent
        {
            return new ControlBinder<TSource,TControl>(this, control);
        }

        public ISourceObservableBinder<TSource, TArg> Observe<TArg>(Func<TSource, IObservable<TArg>> observableMember)
        {
            return new SourceObservableBinder<TSource,TArg>(this, observableMember);
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

    internal class SourceObservableBinder<TSource, TArg> : ISourceObservableBinder<TSource,TArg> 
        where TSource : INotifyPropertyChanged
    {
        private readonly SourceBinder<TSource> _parent;
        private readonly Func<TSource, IObservable<TArg>> _observableMember;

        public SourceObservableBinder(SourceBinder<TSource> parent, Func<TSource, IObservable<TArg>> observableMember)
        {
            _parent = parent;
            _observableMember = observableMember;
        }

        public ISourceBinder<TSource> OnNext(Action<TArg> action)
        {
            var observable = _observableMember(_parent.Source);
            var subscription = observable.Subscribe(action);
            _parent.AddSubscription(subscription);
            return _parent;
        }
    }
}