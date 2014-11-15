using System;
using System.ComponentModel;
using System.Threading;
using Bindery.Interfaces;
using Bindery.Interfaces.Observables;

namespace Bindery.Implementations
{
    internal class SourceObservableBinder<TSource, TArg> : 
        ISourceObservableBinder<TSource,TArg>, 
        IOnNextDefined<TSource>, 
        IOnErrorDefined<TSource>, 
        IOnCompleteDefined<TSource> 
        where TSource : INotifyPropertyChanged
    {
        private readonly SourceBinder<TSource> _parent;
        private readonly IObservable<TArg> _observable;
        private Action<TArg> _onNext;
        private Action<Exception> _onError;
        private Action _onComplete;
        private CancellationToken _cancellationToken;

        public SourceObservableBinder(SourceBinder<TSource> parent, Func<TSource, IObservable<TArg>> observableMember)
        {
            _parent = parent;
            _observable = observableMember(_parent.Source);
        }

        public IOnNextDefined<TSource> OnNext(Action<TArg> onNext)
        {
            _onNext = onNext;
            return this;
        }

        public IOnErrorDefined<TSource> OnError(Action<Exception> onError)
        {
            _onError = onError;
            return this;
        }

        public IOnCompleteDefined<TSource> OnComplete(Action onComplete)
        {
            _onComplete = onComplete;
            return this;
        }

        public ISubscriptionComplete<TSource> CancellationToken(CancellationToken token)
        {
            _cancellationToken = token;
            return this;
        }

        public ISourceBinder<TSource> Subscribe()
        {
            if (_cancellationToken == default(CancellationToken))
            {
                var subscription = CreateSubscription();
                _parent.AddSubscription(subscription);
            }
            else
            {
                SubscribeWithCancellationToken();
            }
            return _parent;
        }

        private IDisposable CreateSubscription()
        {
            return _onError != null && _onComplete != null
                ? _observable.Subscribe(_onNext, _onError, _onComplete)
                : _onError != null
                    ? _observable.Subscribe(_onNext, _onError)
                    : _onComplete != null
                        ? _observable.Subscribe(_onNext, _onComplete)
                        : _observable.Subscribe(_onNext);
        }

        private void SubscribeWithCancellationToken()
        {
            if (_onError != null && _onComplete != null)
            {
                _observable.Subscribe(_onNext, _onError, _onComplete, _cancellationToken);
                return;
            }
            if (_onError != null)
            {
                _observable.Subscribe(_onNext, _onError, _cancellationToken);
                return;
            }
            if (_onComplete != null)
            {
                _observable.Subscribe(_onNext, _onComplete, _cancellationToken);
                return;
            }
            _observable.Subscribe(_onNext, _cancellationToken);
        }

    }
}