using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using Bindery.Expressions;
using Bindery.Extensions;
using Bindery.Interfaces;

namespace Bindery.Implementations
{
    internal class TargetPropertyBinder<TSource, TTarget, TProp> : ITargetPropertyBinder<TSource,TTarget,TProp> 
    {
        private readonly TargetBinder<TSource, TTarget> _parent;
        private readonly Action<TProp> _propSetter;

        public TargetPropertyBinder(TargetBinder<TSource, TTarget> parent, Expression<Func<TTarget, TProp>> member)
        {
            _parent = parent;
            _propSetter = member.GetPropertySetter(_parent.Target);
        }

        public ITargetBinder<TSource, TTarget> Get(Expression<Func<TSource, TProp>> sourceExpression)
        {
            Action<TProp> propertyUpdater = value => _propSetter(value);
            ConfigureTargetPropertyUpdate(sourceExpression, propertyUpdater);
            return _parent;
        }

        private void ConfigureTargetPropertyUpdate<TSourceProp>(Expression<Func<TSource, TSourceProp>> sourceExpression, Action<TSourceProp> updateProperty)
        {
            // Update target with source value immediately
            var sourceAccessor = sourceExpression.Compile();
            var sourceValue = sourceAccessor(_parent.Source);
            updateProperty(sourceValue);

            // Update target when source property changes
            var notificationSources = new NotifyPropertyChangedExpressionAnalyzer().GetSources(_parent.Source, sourceExpression).ToList();
            if (!notificationSources.Any())
                throw new ArgumentException("At least one object defined in the expression must implement INotifyPropertyChanged.");
            var observables = notificationSources.Select(source => source.Object.CreatePropertyChangedObservable(source.PropertyNames)
                .Select(x => sourceAccessor(_parent.Source)));
            var subscriptions = observables.Select(observable => observable.Subscribe(updateProperty));
            subscriptions.ToList().ForEach(sub => _parent.AddSubscription(sub));
        }
    }
}
