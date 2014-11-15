using System;
using System.ComponentModel;
using System.Linq.Expressions;
using Bindery.Extensions;
using Bindery.Interfaces;

namespace Bindery.Implementations
{
    internal class TargetPropertyBinder<TSource, TTarget, TProp> : ITargetPropertyBinder<TSource,TTarget,TProp> 
        where TSource : INotifyPropertyChanged
    {
        private readonly TargetBinder<TSource, TTarget> _parent;
        private readonly Action<TProp> _propSetter;

        public TargetPropertyBinder(TargetBinder<TSource, TTarget> parent, Expression<Func<TTarget, TProp>> member)
        {
            _parent = parent;
            _propSetter = member.GetPropertySetter(_parent.Target);
        }

        public ITargetBinder<TSource, TTarget> UpdateTargetFrom(Expression<Func<TSource, TProp>> sourceMember)
        {
            // Update target with source value immediately
            var sourceAccessor = sourceMember.Compile();
            var sourceValue = sourceAccessor(_parent.Source);
            _propSetter(sourceValue);
            // Update target when source property changes
            var observable = _parent.GetSourcePropertyChangedValueObservable(sourceMember.GetAccessorName(), sourceAccessor);
            var subscription = observable.Subscribe(value => _propSetter(value));
            _parent.AddSubscription(subscription);
            return _parent;
        }
    }
}
