using Bindery.Implementations;
using Bindery.Interfaces;
using Bindery.Interfaces.Binders;

namespace Bindery
{
    /// <summary>
    ///     Base static factory
    /// </summary>
    public static class Create
    {
        /// <summary>
        ///     Create a binding manager object for a source object
        /// </summary>
        /// <typeparam name="T">The source type</typeparam>
        /// <param name="source">The source</param>
        /// <returns>A source binding manager</returns>
        /// <remarks>Dispose of the source binder to remove bindings and event/observable subscriptions created by it.</remarks>
        public static ISourceBinder<T> Binder<T>(T source)
        {
            return new SourceBinder<T>(source);
        }

        /// <summary>
        ///     Create an event observable factory for an object
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="obj">The object</param>
        /// <returns>An event observable factory</returns>
        public static IEventObservableFactory ObservableFor<T>(T obj)
        {
            return new EventObservableFactory<T>(obj);
        }
    }
}