using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;
using System.Windows.Input;
using Bindery.Extensions;
using Bindery.Interfaces;

namespace Bindery.Implementations
{
    internal class ControlEventBinder<TSource, TControl, TEventArgs> : IControlEventBinder<TSource, TControl, TEventArgs>
        where TSource : INotifyPropertyChanged
        where TControl : IBindableComponent 
    {
        private readonly ControlBinder<TSource, TControl> _parent;
        private readonly IObservable<TEventArgs> _observable;

        public ControlEventBinder(ControlBinder<TSource, TControl> parent, string eventName)
        {
            _parent = parent;
            _observable = EventObservable.For(parent.Control).Create<TEventArgs>(eventName);
        }

        public IControlBinder<TSource, TControl> Executes(Func<TSource, ICommand> commandMember)
        {
            var command = commandMember(_parent.Source);
            var subscription = _observable.Subscribe(x => command.ExecuteIfValid(null));
            _parent.AddSubscription(subscription);
            return _parent;
        }

        public IControlBinder<TSource, TControl> Executes<TConverted>(Func<TSource, ICommand> commandMember, Func<TEventArgs, TConverted> conversion)
        {
            var command = commandMember(_parent.Source);
            var subscription = _observable.Subscribe(x => command.ExecuteIfValid(conversion(x)));
            _parent.AddSubscription(subscription);
            return _parent;
        }

        public IControlBinder<TSource, TControl> UpdateSource<TSourceProp>(Expression<Func<TSource, TSourceProp>> propertyMember, Func<TEventArgs, TSourceProp> conversion)
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