using System;
using System.Linq.Expressions;

namespace Bindery.Interfaces.Binders
{
    public interface ITargetPropertyBinder<TSource, TTarget, TProp>
    {
        ITargetBinder<TSource, TTarget> Get(Expression<Func<TSource, TProp>> sourceExpression);
    }
}