using System;
using System.Windows.Forms;

namespace Bindery.Interfaces.Basic
{
    public interface IBasicSourceBinder<TSource> : IDisposable
    {
        IBasicControlBinder<TSource, TControl> Control<TControl>(TControl control) where TControl : IBindableComponent;
        ISourceObservableBinder<TSource, TArg> Observe<TArg>(Func<TSource, IObservable<TArg>> observableMember);
    }
}