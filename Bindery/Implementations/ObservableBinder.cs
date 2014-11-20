using System;
using System.Linq.Expressions;
using System.Threading;
using System.Windows.Input;
using Bindery.Extensions;
using Bindery.Interfaces;
using Bindery.Interfaces.Observables;

namespace Bindery.Implementations
{
    internal class ObservableBinder<TSource, TArg> : 
        IObservableBinder<TSource,TArg>, 
        IOnNextDefined<TSource>, 
        IOnErrorDefined<TSource>, 
        IOnCompleteDefined<TSource> 
    {
        private readonly SourceBinder<TSource> _parent;
        private readonly IObservable<TArg> _observable;
        private Action<TArg> _onNext;
        private Action<Exception> _onError;
        private Action _onComplete;
        private CancellationToken _cancellationToken;

        public ObservableBinder(SourceBinder<TSource> parent, Func<TSource, IObservable<TArg>> observableMember)
            : this(parent, observableMember(parent.Source))
        {
        }

        public ObservableBinder(SourceBinder<TSource> parent, IObservable<TArg> observable)
        {
            _parent = parent;
            _observable = observable;
        }

        public IOnNextDefined<TSource> OnNext(Action<TArg> onNext)
        {
            _onNext = onNext;
            return this;
        }

        public IObservableBinder<TSource, TOut> Transform<TOut>(Func<IObservable<TArg>, IObservable<TOut>> transform)
        {
            return new ObservableBinder<TSource, TOut>(_parent, transform(_observable));
        }

        public ISourceBinder<TSource> Subscribe(Action<TArg> action)
        {
            _onNext = action;
            return Subscribe();
        }

        public ISourceBinder<TSource> Execute(ICommand command)
        {
            var subscription = _observable.Subscribe(x => command.ExecuteIfValid(null));
            _parent.AddSubscription(subscription);
            return _parent;
        }

        public ISourceBinder<TSource> Execute<TCommandArg>(ICommand command, Func<TArg, TCommandArg> conversion)
        {
            var subscription = _observable.Subscribe(x => command.ExecuteIfValid(conversion(x)));
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

        public ISourceBinder<TSource> Set<TProp>(Expression<Func<TSource, TProp>> member, Func<TArg, TProp> conversion)
        {
            var propertySetter = member.GetPropertySetter(_parent.Source);
            var subscription = _observable.Subscribe(arg => propertySetter(conversion(arg)));
            _parent.AddSubscription(subscription);
            return _parent;
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