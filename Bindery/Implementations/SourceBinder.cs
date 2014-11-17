using System;
using System.ComponentModel;
using System.Linq.Expressions;
using Bindery.Implementations.Basic;
using Bindery.Interfaces;

namespace Bindery.Implementations
{
    internal class SourceBinder<TSource> : BasicSourceBinder<TSource>, ISourceBinder<TSource> 
        where TSource : INotifyPropertyChanged
    {
        public SourceBinder(TSource source) :base(source)
        {
        }

        IControlBinder<TSource, TControl> ISourceBinder<TSource>.Control<TControl>(TControl control)
        {
            return new ControlBinder<TSource, TControl>(this, control);
        }

        public ITargetBinder<TSource, TTarget> Target<TTarget>(TTarget target) where TTarget : class
        {
            return new TargetBinder<TSource, TTarget>(this, target);
        }

        public ISourcePropertyBinder<TSource, TProp> Property<TProp>(Expression<Func<TSource, TProp>> member)
        {
            return new SourcePropertyBinder<TSource, TProp>(this, member);
        }

        ISourceObservableBinder<TSource, TArg> ISourceBinder<TSource>.Observe<TArg>(Func<TSource, IObservable<TArg>> observableMember)
        {
            return new SourceObservableBinder<TSource, TArg>(this, observableMember);
        }
    }
}