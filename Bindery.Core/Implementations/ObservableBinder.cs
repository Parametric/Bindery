using System;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
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
        private IScheduler _scheduler;

        public ObservableBinder(SourceBinder<TSource> parent, IObservable<TArg> observable, IScheduler scheduler)
        {
            _parent = parent;
            _observable = observable;
            _scheduler = scheduler;
        }

        public IObservableBinder<TSource, TOut> Transform<TOut>(Func<IObservable<TArg>, IObservable<TOut>> transform)
        {
            return new ObservableBinder<TSource, TOut>(_parent, transform(_observable), _scheduler);
        }

        public IObservableBinder<TSource, TArg> ObserveOn(IScheduler scheduler)
        {
            _scheduler = scheduler;
            return this;
        }

        public ISourceBinder<TSource> Subscribe(Action<TArg> action)
        {
            var observable = GetObservableForSubscription();
            var subscription = observable.Subscribe(action);
            _parent.RegisterDisposable(subscription);
            return _parent;
        }

        public ISourceBinder<TSource> Subscribe(Func<ISubscriptionContext<TArg>, ISubscriptionComplete> subscription)
        {
            var context = new SubscriptionContext<TArg>();
            subscription(context);
            var observable = GetObservableForSubscription();
            var disposable = context.Subscribe(observable);
            if (disposable != null)
                _parent.RegisterDisposable(disposable);
            return _parent;
        }

        public ISourceBinder<TSource> Execute(ICommand command)
        {
            return Subscribe(x => command.ExecuteIfValid(x));
        }

        public ISourceBinder<TSource> Execute(ICommand command, object commandParameter)
        {
            return Subscribe(x => command.ExecuteIfValid(commandParameter));
        }

        public ISourceBinder<TSource> Execute(ICommand command, Func<object> getCommandParameter)
        {
            return Subscribe(x => command.ExecuteIfValid(getCommandParameter()));
        }

        public ISourceBinder<TSource> Set(Expression<Func<TSource, TArg>> member)
        {
            var propertySetter = member.GetPropertySetter(_parent.Source);
            return Subscribe(propertySetter);
        }

        private IObservable<TArg> GetObservableForSubscription()
        {
            var observable = _observable;
            if (_scheduler != null)
                observable = observable.ObserveOn(_scheduler);
            return observable;
        }
    }
}