using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;
using System.Windows.Input;
using Bindery.Extensions;
using Bindery.Interfaces;

namespace Bindery.Implementations
{
    internal class ControlObservableBinder<TSource,TControl,TArg> : IControlObservableBinder<TSource,TControl,TArg> 
        where TControl : IBindableComponent
        where TSource : INotifyPropertyChanged
    {
        private readonly ControlBinder<TSource, TControl> _parent;
        private readonly IObservable<TArg> _observable;

        public ControlObservableBinder(ControlBinder<TSource, TControl> parent, IObservable<TArg> observable)
        {
            _parent = parent;
            _observable = observable;
        }

        public IControlBinder<TSource, TControl> Executes(Func<TSource, ICommand> commandMember)
        {
            ConfigureCommandExecution(commandMember, x => x);
            return _parent;
        }

        public IControlBinder<TSource, TControl> Executes<TCommandArg>(Func<TSource, ICommand> commandMember, Func<TArg, TCommandArg> conversion)
        {
            ConfigureCommandExecution(commandMember, conversion);
            return _parent;
        }

        private void ConfigureCommandExecution<TCommandArg>(Func<TSource, ICommand> commandMember, Func<TArg, TCommandArg> createCommandArg)
        {
            var command = commandMember(_parent.Source);
            var subscription = _observable.Subscribe(args => command.ExecuteIfValid(createCommandArg(args)));
            _parent.AddSubscription(subscription);
        }

        public IControlBinder<TSource, TControl> UpdateSource(Expression<Func<TSource, TArg>> member)
        {
            var onNext = member.GetPropertySetter(_parent.Source);
            _parent.AddSubscription(_observable.Subscribe(onNext));
            return _parent;
        }

        public IControlBinder<TSource, TControl> UpdateSource<TSourceProp>(Expression<Func<TSource, TSourceProp>> member, Func<TArg,TSourceProp> conversion)
        {
            var propertySetter = member.GetPropertySetter(_parent.Source);
            Action<TArg> onNext = args => propertySetter(conversion(args));
            _parent.AddSubscription(_observable.Subscribe(onNext));
            return _parent;
        }
    }
}
