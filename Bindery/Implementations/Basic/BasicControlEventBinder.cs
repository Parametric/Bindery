using System;
using System.Linq.Expressions;
using System.Windows.Forms;
using System.Windows.Input;
using Bindery.Extensions;
using Bindery.Interfaces;
using Bindery.Interfaces.Basic;

namespace Bindery.Implementations.Basic
{
    internal class BasicControlEventBinder<TSource, TControl, TEventArgs> : IBasicControlEventBinder<TSource, TControl, TEventArgs> 
        where TControl : IBindableComponent
    {
        private readonly BasicControlBinder<TSource, TControl> _parent;
        private readonly IObservable<TEventArgs> _observable;

        public BasicControlEventBinder(BasicControlBinder<TSource, TControl> parent, string eventName)
        {
            _parent = parent;
            _observable = Create.ObservableFor(parent.Control).Event<TEventArgs>(eventName);
        }

        public IBasicControlBinder<TSource, TControl> Execute(ICommand command)
        {
            var subscription = _observable.Subscribe(x => command.ExecuteIfValid(null));
            _parent.AddSubscription(subscription);
            return _parent;
        }

        public IBasicControlBinder<TSource, TControl> Execute<TConverted>(ICommand command, Func<TEventArgs, TConverted> conversion)
        {
            var subscription = _observable.Subscribe(x => command.ExecuteIfValid(conversion(x)));
            _parent.AddSubscription(subscription);
            return _parent;
        }

        public IBasicControlBinder<TSource, TControl> Set<TSourceProp>(Expression<Func<TSource, TSourceProp>> propertyMember, Func<TEventArgs, TSourceProp> conversion)
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