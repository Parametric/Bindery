using System;
using System.Linq.Expressions;

namespace Bindery.Interfaces.Binders
{
    /// <summary>
    ///     Binder for a target property
    /// </summary>
    /// <typeparam name="TSource">Type of the binding source</typeparam>
    /// <typeparam name="TTarget">Type of the binding target</typeparam>
    /// <typeparam name="TProp">Type of the bound target property</typeparam>
    public interface ITargetPropertyBinder<TSource, TTarget, TProp>
    {
        /// <summary>
        ///     Create one-way binding that updates the target's property value from the source
        /// </summary>
        /// <param name="sourceExpression">Expression that specifies the source property</param>
        /// <returns>The target binder</returns>
        ITargetBinder<TSource, TTarget> Get(Expression<Func<TSource, TProp>> sourceExpression);
    }
}