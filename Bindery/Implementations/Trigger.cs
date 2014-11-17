using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using Bindery.Interfaces;

namespace Bindery.Implementations
{
    public class Trigger<T> : ITrigger<T>
    {
        private readonly ConcurrentDictionary<long,IObserver<T>> _observers;
        private long _counter;

        public Trigger()
        {
            _observers = new ConcurrentDictionary<long, IObserver<T>>();
            Observable = System.Reactive.Linq.Observable.Create<T>(o =>
            {
                var key = Interlocked.Increment(ref _counter);
                _observers.TryAdd(key, o); 
                return Disposable.Create(() =>
                {
                    IObserver<T> temp;
                    _observers.TryRemove(key, out temp);
                });
            });
        }

        public IObservable<T> Observable { get; private set; }
        public void Push(T arg)
        {
            _observers.Values.ToList().ForEach(o=>o.OnNext(arg));
        }

        public void Dispose()
        {
            _observers.Values.ToList().ForEach(o => o.OnCompleted());
        }
    }
}