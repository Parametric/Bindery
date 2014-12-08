using System;
using System.Linq.Expressions;
using System.Windows.Forms;
using Bindery.Implementations;

namespace Bindery.Interfaces.Binders
{
    /// <summary>
    ///     Root binder for a binding source
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
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
        ///     Bind to an IBindableComponent
        /// </summary>
        /// <typeparam name="TSource">The source type</typeparam>
        /// <typeparam name="TControl">The control type</typeparam>
        /// <param name="control">The control</param>
        /// <returns>A control binder</returns>
        IControlBinder<TSource, TControl> Control<TControl>(TControl control) where TControl : IBindableComponent;

        /// <summary>
        ///     Observe source property changes
        /// </summary>
        /// <typeparam name="TProp">The type of the property</typeparam>
        /// <param name="member">The source property</param>
        /// <returns>An observable binding for the property value</returns>
        /// <remarks>The source must implement INotifyPropertyChanged properly</remarks>
        IObservableBinder<TSource, TProp> OnPropertyChanged<TProp>(Expression<Func<TSource, TProp>> member);

        /// <summary>
        ///     Bind to an observable
        /// </summary>
        /// <typeparam name="TArg">The observable type</typeparam>
        /// <param name="observable">The observable</param>
        /// <returns>An observable binding</returns>
        IObservableBinder<TSource, TArg> Observe<TArg>(IObservable<TArg> observable);

        /// <summary>
        ///     Register disposables created outside the binder for disposal when the binder is disposed.
        /// </summary>
        /// <param name="disposables">One or more IDisposable objects</param>
        /// <returns>This source binder</returns>
        ISourceBinder<TSource> RegisterDisposable(params IDisposable[] disposables);
    }
}