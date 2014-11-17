using System;
using System.Linq.Expressions;
using System.Windows.Forms;
using System.Windows.Input;
using Bindery.Extensions;
using Bindery.Interfaces;
using Bindery.Interfaces.Basic;

namespace Bindery.Implementations.Basic
{
    internal class BasicControlObservableBinder<TSource, TControl, TArg> : IBasicControlObservableBinder<TSource, TControl, TArg>
        where TControl : IBindableComponent
    {
        private readonly BasicControlBinder<TSource, TControl> _parent;
        private readonly IObservable<TArg> _observable;

        public BasicControlObservableBinder(BasicControlBinder<TSource, TControl> parent, IObservable<TArg> observable)
        {
            _parent = parent;
            _observable = observable;
        }

        public IBasicControlBinder<TSource, TControl> Execute(ICommand command)
        {
            var subscription = _observable.Subscribe(args => command.ExecuteIfValid(args));
            _parent.AddSubscription(subscription);
            return _parent;
        }

        public IBasicControlBinder<TSource, TControl> Execute<TCommandArg>(ICommand command, Func<TArg, TCommandArg> conversion)
        {
            var subscription = _observable.Subscribe(args => command.ExecuteIfValid(conversion(args)));
            _parent.AddSubscription(subscription);
            return _parent;
        }

        public IBasicControlBinder<TSource, TControl> Set(Expression<Func<TSource, TArg>> member)
        {
            var onNext = member.GetPropertySetter(_parent.Source);
            _parent.AddSubscription(_observable.Subscribe(onNext));
            return _parent;
        }

        public IBasicControlBinder<TSource, TControl> Set<TSourceProp>(Expression<Func<TSource, TSourceProp>> member, Func<TArg, TSourceProp> conversion)
        {
            var propertySetter = member.GetPropertySetter(_parent.Source);
            Action<TArg> onNext = args => propertySetter(conversion(args));
            _parent.AddSubscription(_observable.Subscribe(onNext));
            return _parent;
        }

    }
}