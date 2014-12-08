using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows.Forms;
using Bindery.Extensions;
using Bindery.Interfaces.Binders;

namespace Bindery.Implementations
{
    internal class SourceBinder<TSource> : ISourceBinder<TSource>
    {
        private readonly List<IDisposable> _disposables;
        private bool _disposed;

        public SourceBinder(TSource source, IScheduler defaultScheduler)
        {
            Source = source;
            _disposables = new List<IDisposable>();
            DefaultScheduler = defaultScheduler;
        }

        public TSource Source { get; private set; }

        public IScheduler DefaultScheduler { get; private set; }

        public ITargetBinder<TSource, TTarget> Target<TTarget>(TTarget target) where TTarget : class
        {
            return new TargetBinder<TSource, TTarget>(this, target);
        }

        public IControlBinder<TSource, TControl> Control<TControl>(TControl control) where TControl : IBindableComponent
        {
            return new ControlBinder<TSource, TControl>(this, control);
        }

        public IObservableBinder<TSource, TProp> OnPropertyChanged<TProp>(Expression<Func<TSource, TProp>> member)
        {
            var source = Source;
            var notifyPropertyChanged = source as INotifyPropertyChanged;
            if (notifyPropertyChanged == null)
                throw new NotSupportedException();
            var memberAccessor = member.Compile();
            var observable = notifyPropertyChanged.CreatePropertyChangedObservable()
                .Where(args => args.PropertyName == member.GetAccessorName())
                .Select(x => memberAccessor(source));
            return new ObservableBinder<TSource, TProp>(this, observable, DefaultScheduler);
        }

        public IObservableBinder<TSource, TArg> Observe<TArg>(IObservable<TArg> observable)
        {
            return new ObservableBinder<TSource, TArg>(this, observable, DefaultScheduler);
        }

        public ISourceBinder<TSource> RegisterDisposable(params IDisposable[] disposables)
        {
            _disposables.AddRange(disposables);
            return this;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposables.ForEach(x => TryDispose(x));
            _disposed = true;
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