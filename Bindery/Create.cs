using System.ComponentModel;
using Bindery.Implementations;
using Bindery.Implementations.Basic;
using Bindery.Interfaces;
using Bindery.Interfaces.Basic;
using Bindery.Interfaces.Observables;

namespace Bindery
{
    public static class Create
    {
        public static IEventObservableFactory ObservableFor<T>(T obj)
        {
            return new EventObservableFactory<T>(obj);
        }

        public static ISourceBinder<T> Binder<T>(T source) where T : INotifyPropertyChanged
        {
            return new SourceBinder<T>(source);
        }

        public static IBasicSourceBinder<T> BasicBinder<T>(T source)
        {
            return new BasicSourceBinder<T>(source);
        }

    }
}
