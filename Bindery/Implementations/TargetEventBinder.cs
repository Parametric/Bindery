using System;
using System.Linq.Expressions;
using System.Windows.Input;
using Bindery.Extensions;
using Bindery.Interfaces;

namespace Bindery.Implementations
{
    internal class TargetEventBinder<TSource, TControl, TEventArgs> : ITargetEventBinder<TSource, TControl, TEventArgs> 
    {
        private readonly TargetBinder<TSource, TControl> _parent;
        private readonly IObservable<TEventArgs> _observable;

        public TargetEventBinder(TargetBinder<TSource, TControl> parent, string eventName)
        {
            _parent = parent;
            _observable = Create.ObservableFor(parent.Target).Event<TEventArgs>(eventName);
        }

        public ITargetBinder<TSource, TControl> Execute(ICommand command)
        {
            var subscription = _observable.Subscribe(x => command.ExecuteIfValid(null));
            _parent.AddSubscription(subscription);
            return _parent;
        }

        public ITargetBinder<TSource, TControl> Execute<TConverted>(ICommand command, Func<TEventArgs, TConverted> conversion)
        {
            var subscription = _observable.Subscribe(x => command.ExecuteIfValid(conversion(x)));
            _parent.AddSubscription(subscription);
            return _parent;
        }

        public ITargetBinder<TSource, TControl> Set<TSourceProp>(Expression<Func<TSource, TSourceProp>> propertyMember, Func<TEventArgs, TSourceProp> conversion)
        {
            var propertySetter = propertyMember.GetPropertySetter(_parent.Source);
            var subscription = _observable.Subscribe(args => propertySetter(conversion(args)));
            _parent.AddSubscription(subscription);
            return _parent;
        }

        public IObservable<TEventArgs> AsObservable()
        {
            return _observable;
        }
    }
}