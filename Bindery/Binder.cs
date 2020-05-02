using System.Reactive.Concurrency;
using Bindery.Implementations;
using Bindery.Interfaces;
using Bindery.Interfaces.Binders;

namespace Bindery
{
    /// <summary>
    ///     Base static factory
    /// </summary>
    public static class Binder
    {
        /// <summary>
        ///     Create a binding manager object for a source object
        /// </summary>
        /// <typeparam name="T">The source type</typeparam>
        /// <param name="source">The source</param>
        /// <returns>A source binding manager</returns>
        /// <remarks>Dispose of the source binder to remove bindings and event/observable subscriptions created by it.</remarks>
        public static ISourceBinder<T> Source<T>(T source)
        {
            return new SourceBinder<T>(source, null);
        }

        /// <summary>
        ///     Create a binding manager object for a source object
        /// </summary>
        /// <typeparam name="T">The source type</typeparam>
        /// <param name="source">The source</param>
        /// <param name="defaultScheduler">The default scheduler used by the binder when creating subscriptions</param>
        /// <returns>A source binding manager</returns>
        /// <remarks>Dispose of the source binder to remove bindings and event/observable subscriptions created by it.</remarks>
        public static ISourceBinder<T> Source<T>(T source, IScheduler defaultScheduler)
        {
            return new SourceBinder<T>(source, defaultScheduler);
        }

        /// <summary>
        ///     Create an event observable factory for an object
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="obj">The object</param>
        /// <returns>An event observable factory</returns>
        public static IEventObservableFactory Observe<T>(T obj)
        {
            return new EventObservableFactory<T>(obj);
        }
    }
}