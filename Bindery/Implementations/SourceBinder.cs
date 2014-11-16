using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Windows.Forms;
using Bindery.Interfaces;

namespace Bindery.Implementations
{
    internal class SourceBinder<TSource> : ISourceBinder<TSource> where TSource : INotifyPropertyChanged
    {
        public TSource Source { get; private set; }
        private readonly IObservable<PropertyChangedEventArgs> _propertyChangedObservable;

        private readonly List<IDisposable> _subscriptions;

        public SourceBinder(TSource obj)
        {
            Source = obj;
            _propertyChangedObservable = Observable.FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                argsAction => (sender, e) => argsAction(e),
                handler => Source.PropertyChanged += handler,
                handler => Source.PropertyChanged -= handler);
            _subscriptions = new List<IDisposable>();
        }


        public IControlBinder<TSource, TControl> Control<TControl>(TControl control) where TControl : IBindableComponent
        {
            return new ControlBinder<TSource, TControl>(this, control);
        }

        public ITargetBinder<TSource, TTarget> Target<TTarget>(TTarget target) where TTarget : class
        {
            return new TargetBinder<TSource, TTarget>(this, target);
        }

        public ISourcePropertyBinder<TSource, TProp> Property<TProp>(Expression<Func<TSource, TProp>> member)
        {
            return new SourcePropertyBinder<TSource, TProp>(this, member);
        }

        public ISourceObservableBinder<TSource, TArg> Observe<TArg>(Func<TSource, IObservable<TArg>> observableMember)
        {
            return new SourceObservableBinder<TSource, TArg>(this, observableMember);
        }

        public void Dispose()
        {
            _subscriptions.ForEach(x=>x.Dispose());
        }

        public void AddSubscription(IDisposable subscription)
        {
            _subscriptions.Add(subscription);
        }

        public IObservable<TProp> GetPropertyChangedValueObservable<TProp>(string memberName, Func<TSource, TProp> memberAccessor)
        {
            var observable = _propertyChangedObservable
                .Where(args => args.PropertyName == memberName)
                .Select(x => memberAccessor.Invoke(Source));
            return observable;
        }
    }
}