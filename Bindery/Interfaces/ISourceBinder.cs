using System;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace Bindery.Interfaces
{
    public interface ISourceBinder<TSource> : IDisposable
    {
        /// <summary>
        /// Bind to an IBindableComponent
        /// </summary>
        /// <typeparam name="TControl">The control type</typeparam>
        /// <param name="control">The control</param>
        /// <returns>A control binder</returns>
        IControlBinder<TSource, TControl> Control<TControl>(TControl control) where TControl : IBindableComponent;

        /// <summary>
        /// Bind to a target
        /// </summary>
        /// <typeparam name="TTarget">The target type</typeparam>
        /// <param name="target">The target</param>
        /// <returns>A target binder</returns>
        ITargetBinder<TSource, TTarget> Target<TTarget>(TTarget target) where TTarget : class;

        IObservableBinder<TSource, TProp> Property<TProp>(Expression<Func<TSource, TProp>> member);

        IObservableBinder<TSource, TArg> Observe<TArg>(IObservable<TArg> observable);

        IObservableBinder<TSource, TArg> Observe<TArg>(Func<TSource, IObservable<TArg>> observableMember);
    }
}