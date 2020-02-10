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
            return observable
                .Select(e => Observable.Defer(() => asyncAction(e).ToObservable(scheduler)))
                .Concat();
        }
    }
}
