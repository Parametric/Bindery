using System;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Windows.Forms;
using Bindery.Interfaces.Binders;

namespace Bindery.Implementations
{
    internal class TargetBinder<TSource, TTarget> : ITargetBinder<TSource, TTarget>
    {
        private readonly SourceBinder<TSource> _sourceBinder;

        public TargetBinder(SourceBinder<TSource> sourceBinder, TTarget target)
        {
            _sourceBinder = sourceBinder;
            Target = target;
        }

        public TTarget Target { get; private set; }

        public TSource Source => _sourceBinder.Source;

        public IScheduler DefaultScheduler => _sourceBinder.DefaultScheduler;

        public ITargetPropertyBinder<TSource, TTarget, TProp> Property<TProp>(Expression<Func<TTarget, TProp>> member)
        {
            return new TargetPropertyBinder<TSource, TTarget, TProp>(this, member);
        }

        public IObservableBinder<TSource, (object Sender, EventArgs Args)> Event(string eventName)
        {
            var observable = Binder.Observe(Target).Event(eventName);
            return CreateObservableBinder(observable);
        }

        public IObservableBinder<TSource, (object Sender, TEventArgs Args)> Event<TEventArgs>(string eventName)
        {
            var observable = Binder.Observe(Target).Event<TEventArgs>(eventName);
            return CreateObservableBinder(observable);
        }

        public void AddSubscription(IDisposable subscription)
        {
            _sourceBinder.RegisterDisposable(subscription);
        }

        protected ObservableBinder<TSource, T> CreateObservableBinder<T>(IObservable<T> observable)
        {
            return new ObservableBinder<TSource, T>(_sourceBinder, observable, DefaultScheduler);
        }

        #region ISourceBinder implementation

        ITargetBinder<TSource, T> ISourceBinder<TSource>.Target<T>(T target)
        {
            return _sourceBinder.Target(target);
        }

        public IControlBinder<TSource, TControl> Control<TControl>(TControl control) where TControl : IBindableComponent
        {
            return new ControlBinder<TSource, TControl>(_sourceBinder, control);
        }

        IObservableBinder<TSource, TProp> ISourceBinder<TSource>.OnPropertyChanged<TProp>(
            Expression<Func<TSource, TProp>> member)
        {
            return _sourceBinder.OnPropertyChanged(member);
        }

        IObservableBinder<TSource, TArg> ISourceBinder<TSource>.Observe<TArg>(IObservable<TArg> observable)
        {
            return _sourceBinder.Observe(observable);
        }

        ISourceBinder<TSource> ISourceBinder<TSource>.RegisterDisposable(params IDisposable[] disposables)
        {
            return _sourceBinder.RegisterDisposable(disposables);
        }

        void IDisposable.Dispose()
        {
            _sourceBinder.Dispose();
        }

        #endregion

        #region Deprecated
        public IObservableBinder<TSource, (object Sender, EventArgs Args)> OnEvent(string eventName)
        {
            // deprecated
            throw new NotImplementedException();
        }

        public IObservableBinder<TSource, (object Sender, TEventArgs Args)> OnEvent<TEventArgs>()
        {
            // deprecated
            throw new NotImplementedException();
        }

        public IObservableBinder<TSource, (object Sender, TEventArgs Args)> OnEvent<TEventArgs>(string eventName)
        {
            // deprecated
            throw new NotImplementedException();
        }
        #endregion
    }
}