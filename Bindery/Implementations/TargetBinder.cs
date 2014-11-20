using System;
using System.Linq.Expressions;
using Bindery.Interfaces;

namespace Bindery.Implementations
{
    internal class TargetBinder<TSource, TTarget> : ITargetBinder<TSource, TTarget>
    {
        private readonly SourceBinder<TSource> _sourceBinder;

        public TTarget Target { get; private set; }

        public TargetBinder(SourceBinder<TSource> sourceBinder, TTarget target)
        {
            _sourceBinder = sourceBinder;
            Target = target;
        }

        public TSource Source
        {
            get { return _sourceBinder.Source; }
        }

        public ITargetPropertyBinder<TSource, TTarget, TProp> Property<TProp>(Expression<Func<TTarget, TProp>> member)
        {
            return new TargetPropertyBinder<TSource, TTarget, TProp>(this, member);
        }

        public IObservableBinder<TSource, EventArgs> OnEvent(string eventName)
        {
            var observable = Create.ObservableFor(Target).Event(eventName);
            return new ObservableBinder<TSource, EventArgs>(_sourceBinder, observable);
        }

        public IObservableBinder<TSource, TEventArgs> OnEvent<TEventArgs>(string eventName)
        {
            var observable = Create.ObservableFor(Target).Event<TEventArgs>(eventName);
            return new ObservableBinder<TSource, TEventArgs>(_sourceBinder, observable);
        }

        public IObservableBinder<TSource, TArg> Observe<TArg>(Func<TTarget, IObservable<TArg>> observableMember)
        {
            var observable = observableMember(Target);
            return new ObservableBinder<TSource, TArg>(_sourceBinder, observable);
        }

        public void AddSubscription(IDisposable subscription)
        {
            _sourceBinder.AddSubscription(subscription);
        }
    }
}