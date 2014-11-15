using System;
using System.Linq.Expressions;

namespace Bindery.Interfaces
{
    public interface ITargetBinder<TSource, TTarget>
    {
        ITargetPropertyBinder<TSource,TTarget,TProp> Property<TProp>(Expression<Func<TTarget, TProp>> member);
    }
}