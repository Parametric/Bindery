using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Linq;
using Bindery.Interfaces;

namespace Bindery.Implementations
{
    internal class SourcePropertyBinder<TSource, TProp> : ISourcePropertyBinder<TSource, TProp> where TSource : INotifyPropertyChanged
    {
        private readonly SourceBinder<TSource> _sourceBinder;
        private readonly string _memberName;
        private readonly Func<TSource, TProp> _memberAccessor;

        public SourcePropertyBinder(SourceBinder<TSource> sourceBinder, Expression<Func<TSource, TProp>> member)
        {
            _sourceBinder = sourceBinder;
            _memberName = member.GetAccessorName();
            _memberAccessor = member.Compile();
        }

        public ISourceBinder<TSource> OnChanged(Action<TProp> action)
        {
            var subscription = _sourceBinder.PropertyChangedObservable.Where(args => args.PropertyName == _memberName)
                .Select(x => _memberAccessor.Invoke(_sourceBinder.Object))
                .Subscribe(action);
            _sourceBinder.AddSubscription(subscription);
            return _sourceBinder;
        }
    }
}