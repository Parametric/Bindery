using Bindery.Implementations;
using Bindery.Interfaces;
using Bindery.Interfaces.Observables;

namespace Bindery
{
    public static class Create
    {
        public static IEventObservableFactory ObservableFor<T>(T obj)
        {
            return new EventObservableFactory<T>(obj);
        }

        public static ISourceBinder<T> Binder<T>(T source)
        {
            return new SourceBinder<T>(source);
        }
    }
}
