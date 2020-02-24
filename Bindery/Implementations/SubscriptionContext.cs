using System;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using Bindery.Extensions;
using Bindery.Interfaces.Subscriptions;

namespace Bindery.Implementations
{
    internal class SubscriptionContext<TArg> : ISubscriptionContext<TArg>, IOnNextDefined, IOnCompleteDefined, IOnErrorDefined
    {
        private readonly IScheduler _scheduler;

        private CancellationToken _cancellationToken;
        private Action _onComplete = () => { };
        private Action<Exception> _onError = ex => { };
        private Action<TArg> _onNext;
        private Func<TArg, Task> _asyncOnNext;

        public SubscriptionContext(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public IOnErrorDefined OnError(Action<Exception> onError)
        {
            _onError = onError;
            return this;
        }

        public IOnCompleteDefined OnComplete(Action onComplete)
        {
            _onComplete = onComplete;
            return this;
        }

        public ISubscriptionComplete CancellationToken(CancellationToken token)
        {
            _cancellationToken = token;
            return this;
        }

        public IOnNextDefined OnNext(Action<TArg> onNext)
        {
            _onNext = onNext;
            return this;
        }

        public IOnNextDefined OnNextAsync(Func<TArg, Task> onNext)
        {
            _asyncOnNext = onNext;
            return this;
        }

        public IDisposable Subscribe(IObservable<TArg> observable)
        {
            if (_cancellationToken == default(CancellationToken))
            {
                return CreateSubscription(observable);
            }
            SubscribeWithCancellationToken(observable);
            return null;
        }

        private IDisposable CreateSubscription(IObservable<TArg> observable)
        {
            if (_asyncOnNext == null)
                return observable.Subscribe(_onNext, _onError, _onComplete);

            return observable.CreateObservableForAsyncAction(_asyncOnNext, _scheduler)
                .Subscribe(_ => { }, _onError, _onComplete);
        }

        private void SubscribeWithCancellationToken(IObservable<TArg> observable)
        {
            if (_asyncOnNext == null)
            {
                observable.Subscribe(_onNext, _onError, _onComplete, _cancellationToken);
                return;
            }

            observable.CreateObservableForAsyncAction(_asyncOnNext, _scheduler)
                .Subscribe(_ => { }, _onError, _onComplete, _cancellationToken);
        }
    }
}