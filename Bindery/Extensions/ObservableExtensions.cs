using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace Bindery.Extensions
{
    public static class ObservableExtensions
    {
        /// <summary>
        /// Asynchronously post a value to an IObservable that also implements IObserver, such as Subject
        /// </summary>
        /// <typeparam name="T">The value type</typeparam>
        /// <param name="observable">The observable</param>
        /// <param name="value">The value</param>
        /// <param name="synchronizationContext">A synchronization context</param>
        public static void Post<T>(this IObservable<T> observable, T value, SynchronizationContext synchronizationContext)
        {
            if (!(observable is IObserver<T> observer))
                throw new ArgumentException("The observable must also implement IObserver", nameof(observable));
            if (synchronizationContext == null)
                throw new ArgumentNullException(nameof(synchronizationContext));
            synchronizationContext.Post(_ => observer.OnNext(value), null);
        }

        /// <summary>
        /// Send a value to an IObservable that also implements IObserver, such as Subject
        /// </summary>
        /// <typeparam name="T">The value type</typeparam>
        /// <param name="observable">The observable</param>
        /// <param name="value">The value</param>
        public static void Send<T>(this IObservable<T> observable, T value)
        {
            if (!(observable is IObserver<T> observer))
                throw new ArgumentException("The observable must also implement IObserver", nameof(observable));
            observer.OnNext(value);
        }

        /// <summary>
        /// Synchronously send a value to an IObservable that also implements IObserver, such as Subject
        /// </summary>
        /// <typeparam name="T">The value type</typeparam>
        /// <param name="observable">The observable</param>
        /// <param name="value">The value</param>
        /// <param name="synchronizationContext">A synchronization context</param>
        public static void Send<T>(this IObservable<T> observable, T value, SynchronizationContext synchronizationContext)
        {
            if (!(observable is IObserver<T> observer))
                throw new ArgumentException("The observable must also implement IObserver", nameof(observable));
            if (synchronizationContext == null)
                throw new ArgumentNullException(nameof(synchronizationContext));
            synchronizationContext.Send(_ => observer.OnNext(value), null);
        }

        public static IObservable<Unit> CreateObservableForAsyncAction<TArg>(this IObservable<TArg> observable, Func<TArg, Task> asyncAction, IScheduler scheduler)
        {
            Func<TArg, IObservable<Unit>> observableFactory;
            if (scheduler == null)
                observableFactory = e => asyncAction(e).ToObservable();
            else
                observableFactory = e => asyncAction(e).ToObservable(scheduler);

            return observable
                .Select(e => Observable.Defer(() => observableFactory(e)))
                .Concat();
        }
    }
}
