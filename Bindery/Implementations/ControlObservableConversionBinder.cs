using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Input;
using Bindery.Extensions;
using Bindery.Interfaces;

namespace Bindery.Implementations
{
    internal class ControlObservableConversionBinder<TSource, TControl, TEventArgs, TConverted> : IControlObservableConversionBinder<TSource, TControl, TConverted> 
        where TSource : INotifyPropertyChanged 
        where TControl : IBindableComponent
    {
        private readonly ControlBinder<TSource, TControl> _parent;
        private readonly IObservable<TEventArgs> _observable;
        private readonly Func<TEventArgs, TConverted> _conversion;

        public ControlObservableConversionBinder(ControlBinder<TSource, TControl> parent, IObservable<TEventArgs> observable, Func<TEventArgs, TConverted> conversion)
        {
            _parent = parent;
            _observable = observable;
            _conversion = conversion;
        }

        public IControlBinder<TSource, TControl> Executes(Func<TSource, ICommand> commandMember)
        {
            var command = commandMember(_parent.Source);
            var subscription = _observable.Subscribe(args =>
            {
                var parameter = _conversion(args);
                command.ExecuteIfValid(parameter);
            });
            _parent.AddSubscription(subscription);
            return _parent;
        }

        public IControlBinder<TSource, TControl> UpdateSource(Expression<Func<TSource, TConverted>> member)
        {
            var propertyInfo = typeof(TSource).GetProperty(member.GetAccessorName());
            var subscription = _observable.Subscribe(args => propertyInfo.SetValue(_parent.Source, _conversion(args)));
            _parent.AddSubscription(subscription);
            return _parent;
        }
    }
}