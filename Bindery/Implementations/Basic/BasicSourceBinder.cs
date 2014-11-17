using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Bindery.Interfaces;
using Bindery.Interfaces.Basic;

namespace Bindery.Implementations.Basic
{
    internal class BasicSourceBinder<TSource> : IBasicSourceBinder<TSource>
    {
        private readonly List<IDisposable> _subscriptions;
        private bool _disposed;

        public BasicSourceBinder(TSource source)
        {
            Source = source;
            _subscriptions = new List<IDisposable>();
        }

        public TSource Source { get; private set; }

        public IBasicControlBinder<TSource, TControl> Control<TControl>(TControl control) where TControl : IBindableComponent
        {
            return new BasicControlBinder<TSource, TControl>(this, control);
        }

        public ISourceObservableBinder<TSource, TArg> Observe<TArg>(Func<TSource, IObservable<TArg>> observableMember)
        {
            return new SourceObservableBinder<TSource, TArg>(this, observableMember);
        }

        public void AddSubscription(IDisposable subscription)
        {
            _subscriptions.Add(subscription);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _subscriptions.ForEach(x => x.Dispose());
            _disposed = true;
        }
    }
}