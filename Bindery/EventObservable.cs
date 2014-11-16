using Bindery.Implementations;
using Bindery.Interfaces.Observables;

namespace Bindery
{
    public static class EventObservable
    {
        public static IEventObservableFactory For<T>(T obj)
        {
            return new EventObservableFactory<T>(obj);
        }
    }
}
