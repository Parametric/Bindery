using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Linq;
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

        public ITargetBinder<TSource, TTarget> Get(Expression<Func<TSource, TProp>> sourceMember)
        {
            Action<TProp> propertyUpdater = value => _propSetter(value);
            ConfigureTargetPropertyUpdate(sourceMember, propertyUpdater);
            return _parent;
        }

        public ITargetBinder<TSource, TTarget> Get<TSourceProp>(Expression<Func<TSource, TSourceProp>> sourceMember, Func<TSourceProp, TProp> conversion)
        {
            Action<TSourceProp> propertyUpdater = value => _propSetter(conversion(value));
            ConfigureTargetPropertyUpdate(sourceMember, propertyUpdater);
            return _parent;
        }

        private void ConfigureTargetPropertyUpdate<TSourceProp>(Expression<Func<TSource, TSourceProp>> sourceMember, Action<TSourceProp> updateProperty)
        {
            var sourcePropertyName = sourceMember.GetAccessorName();
            // Update target with source value immediately
            var sourceAccessor = sourceMember.Compile();
            var sourceValue = sourceAccessor(_parent.Source);
            updateProperty(sourceValue);
            // Update target when source property changes
            var observable = CreateObservable(sourceAccessor, sourcePropertyName);
            var subscription = observable.Subscribe(updateProperty);
            _parent.AddSubscription(subscription);
        }

        private IObservable<TSourceProp> CreateObservable<TSourceProp>(Func<TSource, TSourceProp> sourceAccessor, string sourcePropertyName)
        {
            var notifyPropertyChanged = _parent.Source as INotifyPropertyChanged;
            if (notifyPropertyChanged == null)
                throw new NotSupportedException(string.Format("Source type '{0}' does not implement {1}.",
                    typeof (TSource), typeof (INotifyPropertyChanged)));
            return notifyPropertyChanged.CreatePropertyChangedObservable()
                .Where(args => args.PropertyName == sourcePropertyName)
                .Select(x => sourceAccessor(_parent.Source));
        }
    }
}
