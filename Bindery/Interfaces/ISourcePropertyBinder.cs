using System;

namespace Bindery.Interfaces
{
    public interface ISourcePropertyBinder<TSource, out TProp>
    {
        ISourceBinder<TSource> OnChanged(Action<TProp> action);
        IObservable<TProp> AsObservable();
    }
}