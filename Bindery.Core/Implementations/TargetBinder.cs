using System;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using Bindery.Interfaces.Binders;

namespace Bindery.Implementations
{
    internal class TargetBinder<TSource, TTarget> : ITargetBinder<TSource, TTarget>
    {
        private readonly SourceBinder<TSource> _sourceBinder;

        public TargetBinder(SourceBinder<TSource> sourceBinder, TTarget target)
        {
            _sourceBinder = sourceBinder;
            Target = target;
        }

        public TTarget Target { get; private set; }

        public TSource Source
        {
            get { return _sourceBinder.Source; }
        }

        public IScheduler DefaultScheduler
        {
            get { return _sourceBinder.DefaultScheduler; }
        }

        public ITargetPropertyBinder<TSource, TTarget, TProp> Property<TProp>(Expression<Func<TTarget, TProp>> member)
        {
            return new TargetPropertyBinder<TSource, TTarget, TProp>(this, member);
        }

        public IObservableBinder<TSource, EventArgs> OnEvent(string eventName)
        {
            var observable = Create.ObservableFor(Target).Event(eventName);
            return new ObservableBinder<TSource, EventArgs>(_sourceBinder, observable, DefaultScheduler);
        }

        public IObservableBinder<TSource, TEventArgs> OnEvent<TEventArgs>(string eventName)
        {
            var observable = Create.ObservableFor(Target).Event<TEventArgs>(eventName);
            return new ObservableBinder<TSource, TEventArgs>(_sourceBinder, observable, _sourceBinder.DefaultScheduler);
        }

        public void AddSubscription(IDisposable subscription)
        {
            _sourceBinder.AddSubscription(subscription);
        }
    }
}