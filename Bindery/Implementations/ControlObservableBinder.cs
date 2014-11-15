using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Bindery.Implementations
{
    internal class ControlObservableBinder<TSource,TControl,TArg> : ControlObservableConversionBinder<TSource,TControl,TArg,TArg> 
        where TControl : IBindableComponent
        where TSource : INotifyPropertyChanged
    {
        public ControlObservableBinder(ControlBinder<TSource, TControl> parent, IObservable<TArg> observable) : base(parent, observable, x=>x)
        {
        }
    }
}
