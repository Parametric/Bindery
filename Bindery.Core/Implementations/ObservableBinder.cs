using System;
using System.Linq.Expressions;
using System.Windows.Input;
using Bindery.Extensions;
using Bindery.Interfaces.Binders;
using Bindery.Interfaces.Subscriptions;

namespace Bindery.Implementations
{
    internal class ObservableBinder<TSource, TArg> : IObservableBinder<TSource, TArg>
    {
        private readonly IObservable<TArg> _observable;
        private readonly SourceBinder<TSource> _parent;

        public ObservableBinder(SourceBinder<TSource> parent, IObservable<TArg> observable)
        {
            _parent = parent;
            _observable = observable;
        }

        public IObservableBinder<TSource, TOut> Transform<TOut>(Func<IObservable<TArg>, IObservable<TOut>> transform)
        {
            return new ObservableBinder<TSource, TOut>(_parent, transform(_observable));
        }

        public ISourceBinder<TSource> Subscribe(Action<TArg> action)
        {
            _parent.AddSubscription(_observable.Subscribe(action));
            return _parent;
        }

        public ISourceBinder<TSource> Subscribe(Func<ISubscriptionContext<TArg>, ISubscriptionComplete> subscription)
        {
            var context = new SubscriptionContext<TArg>();
            subscription(context);
            var disposable = context.Subscribe(_observable);
            if (disposable != null)
                _parent.AddSubscription(disposable);
            return _parent;
        }

        public ISourceBinder<TSource> Execute(ICommand command)
        {
            var subscription = _observable.Subscribe(x => command.ExecuteIfValid(x));
            _parent.AddSubscription(subscription);
            return _parent;
        }

        public ISourceBinder<TSource> Execute(ICommand command, object commandParameter)
        {
            var subscription = _observable.Subscribe(x => command.ExecuteIfValid(commandParameter));
            _parent.AddSubscription(subscription);
            return _parent;
        }

        public ISourceBinder<TSource> Execute(ICommand command, Func<object> getCommandParameter)
        {
            var subscription = _observable.Subscribe(x => command.ExecuteIfValid(getCommandParameter()));
            _parent.AddSubscription(subscription);
            return _parent;
        }

        public ISourceBinder<TSource> Set(Expression<Func<TSource, TArg>> member)
        {
            var propertySetter = member.GetPropertySetter(_parent.Source);
            var subscription = _observable.Subscribe(propertySetter);
            _parent.AddSubscription(subscription);
            return _parent;
        }
    }
}