﻿using System;
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

        public ITargetBinder<TSource, TTarget> Source(Expression<Func<TSource, TProp>> sourceMember)
        {
            Action<TProp> propertyUpdater = value => _propSetter(value);
            ConfigureTargetPropertyUpdate(sourceMember, propertyUpdater);
            return _parent;
        }

        public ITargetBinder<TSource, TTarget> Source<TSourceProp>(Expression<Func<TSource, TSourceProp>> sourceMember, Func<TSourceProp, TProp> conversion)
        {
            Action<TSourceProp> propertyUpdater = value => _propSetter(conversion(value));
            ConfigureTargetPropertyUpdate(sourceMember, propertyUpdater);
            return _parent;
        }

        private void ConfigureTargetPropertyUpdate<TSourceProp>(Expression<Func<TSource, TSourceProp>> sourceMember, Action<TSourceProp> updateProperty)
        {
            // Update target with source value immediately
            var sourceAccessor = sourceMember.Compile();
            var sourceValue = sourceAccessor(_parent.Source);
            updateProperty(sourceValue);
            // Update target when source property changes
            var observable = _parent.GetSourcePropertyChangedValueObservable(sourceMember.GetAccessorName(), sourceAccessor);
            var subscription = observable.Subscribe(updateProperty);
            _parent.AddSubscription(subscription);
        }
    }
}
