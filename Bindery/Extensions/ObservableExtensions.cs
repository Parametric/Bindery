using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace Bindery.Extensions
{
    public static class ObservableExtensions
    {
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
