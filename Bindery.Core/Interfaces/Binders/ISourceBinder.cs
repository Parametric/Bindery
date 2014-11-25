using System;
using System.Linq.Expressions;

namespace Bindery.Interfaces.Binders
{
    public interface ISourceBinder<TSource> : IDisposable
    {
        /// <summary>
        ///     Bind to a target
        /// </summary>
        /// <typeparam name="TTarget">The target type</typeparam>
        /// <param name="target">The target</param>
        /// <returns>A target binder</returns>
        ITargetBinder<TSource, TTarget> Target<TTarget>(TTarget target) where TTarget : class;

        /// <summary>
        ///     Bind to a source property, where the source implements INotifyPropertyChanged
        /// </summary>
        /// <typeparam name="TProp">The type of the property</typeparam>
        /// <param name="member">The source property</param>
        /// <returns>An observable binding for the property value</returns>
        IObservableBinder<TSource, TProp> Property<TProp>(Expression<Func<TSource, TProp>> member);

        /// <summary>
        ///     Bind to an observable
        /// </summary>
        /// <typeparam name="TArg">The observable type</typeparam>
        /// <param name="observable">The observable</param>
        /// <returns>An observable binding</returns>
        IObservableBinder<TSource, TArg> Observe<TArg>(IObservable<TArg> observable);
    }
}