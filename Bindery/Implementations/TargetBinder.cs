using System;
using System.ComponentModel;
using System.Linq.Expressions;
using Bindery.Interfaces;

namespace Bindery.Implementations
{
    internal class TargetBinder<TSource, TTarget> : ITargetBinder<TSource, TTarget>
        where TSource : INotifyPropertyChanged
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

        public void AddSubscription(IDisposable subscription)
        {
            _sourceBinder.AddSubscription(subscription);
        }
    }
}