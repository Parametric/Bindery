using System;
using System.Linq.Expressions;

namespace Bindery.Interfaces
{
    public interface ITargetPropertyBinder<TSource, TTarget, TProp>
    {
        ITargetBinder<TSource, TTarget> UpdateTargetFrom(Expression<Func<TSource, TProp>> sourceMember);
        ITargetBinder<TSource, TTarget> UpdateTargetFrom<TSourceProp>(Expression<Func<TSource, TSourceProp>> sourceMember, Func<TSourceProp, TProp> conversion);
    }
}