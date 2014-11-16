using System;
using System.ComponentModel;
using System.Linq.Expressions;
using Bindery.Extensions;
using Bindery.Interfaces;

namespace Bindery.Implementations
{
    internal class SourcePropertyBinder<TSource, TProp> : ISourcePropertyBinder<TSource, TProp> where TSource : INotifyPropertyChanged
    {
        private readonly SourceBinder<TSource> _sourceBinder;
        private readonly Expression<Func<TSource, TProp>> _member;

        public SourcePropertyBinder(SourceBinder<TSource> sourceBinder, Expression<Func<TSource, TProp>> member)
        {
            _sourceBinder = sourceBinder;
            _member = member;
        }

        public ISourceBinder<TSource> OnChanged(Action<TProp> action)
        {
            var observable = _sourceBinder.GetPropertyChangedValueObservable(_member.GetAccessorName(), _member.Compile());
            var subscription = observable.Subscribe(action);
            _sourceBinder.AddSubscription(subscription);
            return _sourceBinder;
        }
    }
}