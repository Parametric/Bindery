using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Bindery.Interfaces;

namespace Bindery.Implementations
{
    internal class SourceBinder<TSource> : ISourceBinder<TSource>
    {
        public TSource Source { get; private set; }
        private readonly List<IDisposable> _subscriptions;
        private bool _disposed;

        public SourceBinder(TSource source)
        {
            Source = source;
            _subscriptions = new List<IDisposable>();
        }

        IControlBinder<TSource, TControl> ISourceBinder<TSource>.Control<TControl>(TControl control)
        {
            return new ControlBinder<TSource, TControl>(this, control);
        }

        public ITargetBinder<TSource, TTarget> Target<TTarget>(TTarget target) where TTarget : class
        {
            return new TargetBinder<TSource, TTarget>(this, target);
        }

        public ISourcePropertyBinder<TSource, TProp> Property<TProp>(Expression<Func<TSource, TProp>> member)
        {
            return new SourcePropertyBinder<TSource, TProp>(this, member);
        }

        IObservableBinder<TSource, TArg> ISourceBinder<TSource>.Observe<TArg>(IObservable<TArg> observable)
        {
            return new ObservableBinder<TSource, TArg>(this, observable);
        }

        IObservableBinder<TSource, TArg> ISourceBinder<TSource>.Observe<TArg>(Func<TSource, IObservable<TArg>> observableMember)
        {
            return new ObservableBinder<TSource, TArg>(this, observableMember);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _subscriptions.ForEach(x => TryDispose(x));
            _disposed = true;
        }

        public void AddSubscription(IDisposable subscription)
        {
            _subscriptions.Add(subscription);
        }

        private static bool TryDispose(IDisposable x)
        {
            try
            {
                x.Dispose();
                return true;
            }
            catch (Exception)
            {
                // Disposal can fail accessing old window handles
                return false;
            }
        }
    }
}