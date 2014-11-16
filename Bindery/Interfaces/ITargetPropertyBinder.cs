using System;
using System.Linq.Expressions;

namespace Bindery.Interfaces
{
    public interface ITargetPropertyBinder<TSource, TTarget, TProp>
    {
        ITargetBinder<TSource, TTarget> Source(Expression<Func<TSource, TProp>> sourceMember);
        ITargetBinder<TSource, TTarget> Source<TSourceProp>(Expression<Func<TSource, TSourceProp>> sourceMember, Func<TSourceProp, TProp> conversion);
    }
}