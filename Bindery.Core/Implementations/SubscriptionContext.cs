using System;
using System.Threading;
using Bindery.Interfaces.Subscriptions;

namespace Bindery.Implementations
{
    internal class SubscriptionContext<TArg> : ISubscriptionContext<TArg>, IOnNextDefined, IOnCompleteDefined, IOnErrorDefined
    {
        private CancellationToken _cancellationToken;
        private Action _onComplete;
        private Action<Exception> _onError;
        private Action<TArg> _onNext;

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
            return _onError != null && _onComplete != null
                ? observable.Subscribe(_onNext, _onError, _onComplete)
                : _onError != null
                    ? observable.Subscribe(_onNext, _onError)
                    : _onComplete != null
                        ? observable.Subscribe(_onNext, _onComplete)
                        : observable.Subscribe(_onNext);
        }

        private void SubscribeWithCancellationToken(IObservable<TArg> observable)
        {
            if (_onError != null && _onComplete != null)
            {
                observable.Subscribe(_onNext, _onError, _onComplete, _cancellationToken);
                return;
            }
            if (_onError != null)
            {
                observable.Subscribe(_onNext, _onError, _cancellationToken);
                return;
            }
            if (_onComplete != null)
            {
                observable.Subscribe(_onNext, _onComplete, _cancellationToken);
                return;
            }
            observable.Subscribe(_onNext, _cancellationToken);
        }
    }
}