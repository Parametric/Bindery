using System;
using System.Linq.Expressions;

namespace Bindery.Interfaces
{
    public interface ITargetPropertyBinder<TSource, TTarget, TProp>
    {
        ITargetBinder<TSource, TTarget> UpdateTargetFrom(Expression<Func<TSource, TProp>> sourceMember);
    }
}