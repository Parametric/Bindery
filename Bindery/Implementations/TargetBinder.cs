using System;
using System.Linq.Expressions;
using Bindery.Implementations.Basic;
using Bindery.Interfaces;

namespace Bindery.Implementations
{
    internal class TargetBinder<TSource, TTarget> : ITargetBinder<TSource, TTarget>
    {
        private readonly BasicSourceBinder<TSource> _sourceBinder;

        public TTarget Target { get; private set; }

        public TargetBinder(BasicSourceBinder<TSource> sourceBinder, TTarget target)
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

        public ITargetEventBinder<TSource, TTarget, EventArgs> OnEvent(string eventName)
        {
            return new TargetEventBinder<TSource, TTarget, EventArgs>(this, eventName);
        }

        public ITargetEventBinder<TSource, TTarget, TEventArgs> OnEvent<TEventArgs>(string eventName)
        {
            return new TargetEventBinder<TSource, TTarget, TEventArgs>(this, eventName);
        }

        public void AddSubscription(IDisposable subscription)
        {
            _sourceBinder.AddSubscription(subscription);
        }
    }
}